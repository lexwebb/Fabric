using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Couchbase.Lite;
using Couchbase.Lite.Support;
using Newtonsoft.Json;

namespace Fabric.Data {
    public class CouchbaseDatabaseHelper : IDatabaseHelper {
        public const string NameSeperator = "::";
        public const string DataObjectName = "data";

        public CouchbaseDatabaseHelper(FabricDatabase database, JsonSerializerSettings serializerSettings) {
            SerializerSettings = serializerSettings;
            DatabaseRoot = database.DatabaseRoot;
        }

        public Database CouchbaseDatabase { get; private set; }

        public JsonSerializerSettings SerializerSettings { get; }

        public string DatabaseRoot { get; }

        /// <inheritdoc />
        public bool Initialise() {
            var isNew = !Directory.Exists(DatabaseRoot) || 
                        (!Directory.GetFiles(DatabaseRoot).Any() && !Directory.GetDirectories(DatabaseRoot).Any());

            CouchbaseDatabase = new Database(FabricDatabase.DatabaseName, new DatabaseConfiguration {
                Directory = DatabaseRoot
            });

            return isNew;
        }

        /// <inheritdoc />
        public void WritePage(DataPage page) {
            var path = Utils.GetDataPagePath(page);

            if (page.ParentPage == null && page.Name == "root") {
                path = "root";
            }

            var pagePathSaveName = ConvertPagePathToSaveName(path);

            using (var doc = CouchbaseDatabase.GetDocument(pagePathSaveName)) {
                var mutableDoc = doc?.ToMutable() ?? new MutableDocument(pagePathSaveName);
                mutableDoc.SetString("data", JsonConvert.SerializeObject(page, SerializerSettings));
                CouchbaseDatabase.Save(mutableDoc, ConcurrencyControl.LastWriteWins);
            }
        }

        /// <inheritdoc />
        public DataPage ReadPage(string path) {
            if (string.IsNullOrEmpty(path))
                path = "root";

            var pagePathSaveName = ConvertPagePathToSaveName(path);

            using (var doc = CouchbaseDatabase.GetDocument(pagePathSaveName)) {
                var mutableDoc = doc?.ToMutable();

                if (mutableDoc != null) {
                    return JsonConvert.DeserializeObject<DataPage>(mutableDoc.GetString("data"), SerializerSettings);
                }

                throw new ItemNotFoundException(path);
            }
        }

        /// <inheritdoc />
        public void DeletePage(string path) {
            using (var doc = CouchbaseDatabase.GetDocument(ConvertPagePathToSaveName(path))) {
                CouchbaseDatabase.Delete(doc ?? throw new ItemNotFoundException(path));
            }
        }

        /// <inheritdoc />
        public void WriteSchema(DataPageSchema schema) {
            var schemaSaveName = ConvertSchemaNameToSaveName(schema.SchemaName);

            using (var doc = CouchbaseDatabase.GetDocument(schemaSaveName)) {
                var mutableDoc = doc?.ToMutable() ?? new MutableDocument(schemaSaveName);
                mutableDoc.SetString("data", schema.SchemaRaw);
                CouchbaseDatabase.Save(mutableDoc, ConcurrencyControl.LastWriteWins);
            }
        }

        /// <inheritdoc />
        public DataPageSchema ReadSchema(string schemaName) {
            var schemaSaveName = ConvertSchemaNameToSaveName(schemaName);

            using (var doc = CouchbaseDatabase.GetDocument(schemaSaveName)) {
                var mutableDoc = doc?.ToMutable();

                if (mutableDoc != null) {
                    return new DataPageSchema(schemaName, mutableDoc.GetString("data"));
                }

                throw new ItemNotFoundException(schemaName);
            }
        }

        /// <inheritdoc />
        public void DeleteSchema(string schemaName) {
            var schemaSaveName = ConvertSchemaNameToSaveName(schemaName);

            using (var doc = CouchbaseDatabase.GetDocument(schemaSaveName)) {
                CouchbaseDatabase.Delete(doc ?? throw new ItemNotFoundException(schemaName));
            }
        }

        /// <inheritdoc />
        public void WritePageVersion(DataPageVersion dataPageVersion) {
            var pageLogSaveName = ConvertPagePathToVersionLogSaveName(dataPageVersion.DataPagePath);

            using (var doc = CouchbaseDatabase.GetDocument(pageLogSaveName)) {
                var mutableDoc = doc?.ToMutable() ?? new MutableDocument(pageLogSaveName);
                mutableDoc.SetString("data", JsonConvert.SerializeObject(dataPageVersion, SerializerSettings));
                CouchbaseDatabase.Save(mutableDoc, ConcurrencyControl.LastWriteWins);
            }
        }

        /// <inheritdoc />
        public DataPageVersion ReadPageVersion(string path, Guid versionGuid) {
            var pageVersionSaveName = ConvertPagePathToVersionSaveName(path, versionGuid);

            using (var doc = CouchbaseDatabase.GetDocument(pageVersionSaveName)) {
                var mutableDoc = doc?.ToMutable();

                if (mutableDoc != null) {
                    return JsonConvert.DeserializeObject<DataPageVersion>(mutableDoc.GetString("data"));
                }

                throw new ItemNotFoundException(path, versionGuid.ToString());
            }
        }

        /// <inheritdoc />
        public IEnumerable<DataPageVersion> GetPageVersions(DataPage page) {
            var path = Utils.GetDataPagePath(page);

            // We use EndsWith here because the method 'ConvertPagePathToVersionSaveName'
            // puts the page name at the end of the string. This will also ensure that no
            // longer nested path will be found as the EndsWith ensures the key terminates 
            // at the end of our path.
            var indexes = CouchbaseDatabase.GetIndexes().Where(i => i.EndsWith(path));

            foreach (var index in indexes) {
                yield return GetPageVersion(page,
                    Guid.Parse(index.Split(new[] { NameSeperator }, StringSplitOptions.RemoveEmptyEntries).Last()));
            }
        }

        /// <inheritdoc />
        public DataPageVersion GetPageVersion(DataPage page, Guid versionGuid) {
            var path = Utils.GetDataPagePath(page);
            var pageVersionPath = ConvertPagePathToVersionSaveName(path, versionGuid);

            using (var doc = CouchbaseDatabase.GetDocument(pageVersionPath)) {
                var mutableDoc = doc?.ToMutable();

                if (mutableDoc != null) {
                    return JsonConvert.DeserializeObject<DataPageVersion>(mutableDoc.GetString("data"));
                }

                throw new ItemNotFoundException(path);
            }
        }

        /// <inheritdoc />
        public void WriteSchemaVersion(DataPageSchema dataPage) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public DataPageSchema ReadSchemaVersion(string path, Guid versionGuid) {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Converts a page path to the save name that will be used as the database key.
        /// </summary>
        /// <param name="pagePath"></param>
        /// <returns></returns>
        public string ConvertPagePathToSaveName(string pagePath) {
            return $"page{NameSeperator}{pagePath}";
        }

        /// <summary>
        ///     Converts a page path to the save name that will be used as the database key.
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="versionGuid"></param>
        /// <returns></returns>
        public string ConvertPagePathToVersionSaveName(string pagePath, Guid versionGuid) {
            return $"pageVersion{NameSeperator}{versionGuid.ToString()}{NameSeperator}{pagePath}";
        }

        /// <summary>
        ///     Converts a page path to the save name that will be used as the database key.
        /// </summary>
        /// <param name="pagePath"></param>
        /// <returns></returns>
        public string ConvertPagePathToVersionLogSaveName(string pagePath) {
            return $"pageVersionLog{NameSeperator}{pagePath}";
        }

        /// <summary>
        ///     Converts a schema name to the save name that will be used as the database key.
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public string ConvertSchemaNameToSaveName(string schemaName) {
            return $"schema{NameSeperator}{schemaName}";
        }

        /// <summary>
        ///     Converts a schema name to the save name that will be used as the database key.
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="versionGuid"></param>
        /// <returns></returns>
        public string ConvertSchemaNameToVersionSaveName(string schemaName, Guid versionGuid) {
            return $"schemaVersion{NameSeperator}{versionGuid.ToString()}{NameSeperator}{schemaName}";
        }

        /// <summary>
        ///     Converts a schema name to the save name that will be used as the database key.
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public string ConvertSchemaNameToVersionLogSaveName(string schemaName) {
            return $"schemaVersionLog{NameSeperator}{schemaName}";
        }

        public void Dispose() {
            CouchbaseDatabase?.Dispose();
        }
    }
}
