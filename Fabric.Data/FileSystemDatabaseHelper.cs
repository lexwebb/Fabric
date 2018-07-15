using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fabric.Data {
    public class FileSystemDatabaseHelper : IDatabaseHelper {
        private const string SchemaSubFolderName = "schemas";
        public const string DatabaseFileName = FabricDatabase.DatabaseName + ".json";
        public const string DataPageFileName = "dataPage.json";
        public const string DataPageVersionDirName = ".versions";
        public const string DataPageVersionFileName = "versions.dat";

        public FileSystemDatabaseHelper(FabricDatabase database, JsonSerializerSettings serializerSettings) {
            DatabaseRoot = database.DatabaseRoot;
            FullDatabaseRoot = database.FullDataBaseRoot;
            SerializerSettings = serializerSettings;
        }

        public string DatabaseRoot { get; }

        public string FullDatabaseRoot { get; }

        public string SchemaSubFolderPath => Path.Combine(FullDatabaseRoot, SchemaSubFolderName);

        public JsonSerializerSettings SerializerSettings { get; }

        public string VersionsRootPath => Path.Combine(DatabaseRoot, DataPageVersionDirName);

        public string VersionsFilePath => Path.Combine(VersionsRootPath, DataPageVersionFileName);

        /// <inheritdoc />
        public bool Initialise() {
            if (File.Exists(Path.Combine(FullDatabaseRoot, DatabaseFileName))) {
                return false;
            }

            Directory.CreateDirectory(FullDatabaseRoot);
            Directory.CreateDirectory(Path.Combine(FullDatabaseRoot, DataPageVersionDirName));
            File.OpenWrite(Path.Combine(FullDatabaseRoot, DataPageVersionDirName, DataPageVersionFileName)).Dispose();
            ;
            return true;
        }

        /// <inheritdoc />
        public void WritePage(DataPage page) {
            var path = Utils.GetDataPagePath(page);

            if (string.IsNullOrEmpty(path)) {
                path = DatabaseFileName;
            }
            else {
                path = Path.Combine(path, DataPageFileName);
            }

            WriteFile(Path.Combine(DatabaseRoot, path), JsonConvert.SerializeObject(page, SerializerSettings));
        }

        /// <inheritdoc />
        public DataPage ReadPage(string path) {
            if (string.IsNullOrEmpty(path)) {
                path = DatabaseFileName;
            } else {
                path = Path.Combine(path, DataPageFileName);
            }

            return JsonConvert.DeserializeObject<DataPage>(ReadFile(path), SerializerSettings);
        }

        /// <inheritdoc />
        public void DeletePage(string path) {
            DeleteFolder(path);
        }

        /// <inheritdoc />
        public void WriteSchema(DataPageSchema schema) {
            WriteFile(Path.Combine(SchemaSubFolderPath, $"{schema.SchemaName}.json"),
                JsonUtils.Prettify(schema.SchemaRaw));
        }

        /// <inheritdoc />
        public DataPageSchema ReadSchema(string schemaName) {
            var json = ReadFile(Path.Combine(SchemaSubFolderPath, $"{schemaName}.json"));
            return new DataPageSchema(schemaName, json);
        }

        /// <inheritdoc />
        public void DeleteSchema(string schemaName) {
            DeleteFile(Path.Combine(SchemaSubFolderPath, $"{schemaName}.json"));
        }

        /// <inheritdoc />
        public void WritePageVersion(DataPageVersion dataPageVersion) {
            var versionJson = JObject.FromObject(dataPageVersion);

            var value = versionJson.ToString(Formatting.None);
            AppendFile(VersionsFilePath, $"{dataPageVersion.DataPagePath}:{value}");
            WriteFile(Path.Combine(VersionsRootPath, $"{dataPageVersion.VersionGuid}.json"),
                JsonConvert.SerializeObject(dataPageVersion.DataPage, SerializerSettings));
        }

        /// <inheritdoc />
        public DataPageVersion ReadPageVersion(string path, Guid versionGuid) {
            using (var fileStream = OpenFileStream(VersionsFilePath)) {
                using (var streamReader = new StreamReader(fileStream)) {
                    while (!streamReader.EndOfStream) {
                        var line = streamReader.ReadLine();
                        if (line != null && line.StartsWith(path)) {
                            var data = line.Substring(line.IndexOf(":", StringComparison.Ordinal));
                            var version = JsonConvert.DeserializeObject<DataPageVersion>(data);
                            if (version.VersionGuid == versionGuid) {
                                return version;
                            }
                        }
                    }
                }
            }

            throw new ItemNotFoundException(versionGuid.ToString(), path);
        }

        /// <inheritdoc />
        public IEnumerable<DataPageVersion> GetPageVersions(DataPage page) {
            using (var fileStream = OpenFileStream(VersionsFilePath)) {
                using (var streamReader = new StreamReader(fileStream)) {
                    while (!streamReader.EndOfStream) {
                        var line = streamReader.ReadLine();
                        if (line != null && line.StartsWith(Utils.GetDataPagePath(page))) {
                            var data = line.Substring(line.IndexOf(":", StringComparison.Ordinal) + 1);
                            yield return JsonConvert.DeserializeObject<DataPageVersion>(data, SerializerSettings);
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public DataPageVersion GetPageVersion(DataPage page, Guid versionGuid) {
            using (var fileStream = OpenFileStream(VersionsFilePath)) {
                using (var streamReader = new StreamReader(fileStream)) {
                    while (!streamReader.EndOfStream) {
                        var line = streamReader.ReadLine();
                        if (line != null && line.StartsWith(Utils.GetDataPagePath(page))) {
                            var data = line.Substring(line.IndexOf(":", StringComparison.Ordinal));
                            var version = JsonConvert.DeserializeObject<DataPageVersion>(data, SerializerSettings);
                            if (version.VersionGuid == versionGuid) {
                                return version;
                            }
                        }
                    }
                }
            }

            throw new ItemNotFoundException(versionGuid.ToString(), Utils.GetDataPagePath(page));
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
        ///     Qualifies the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private string QualifyPath(string path) {
            return !path.StartsWith(DatabaseRoot) ? Path.Combine(DatabaseRoot, path) : path;
        }

        /// <summary>
        ///     Appends the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="data">The data.</param>
        private void AppendFile(string path, string data) {
            path = QualifyPath(path);

            File.AppendAllLines(path, new[] {data});
        }

        /// <summary>
        ///     Deletes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        private void DeleteFile(string path) {
            path = QualifyPath(path);

            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        /// <summary>
        ///     Writes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="data">The data.</param>
        private void WriteFile(string path, string data = null) {
            path = QualifyPath(path);

            CreateFolder(Path.GetDirectoryName(path));
            var stream = File.Exists(path) ? File.Open(path, FileMode.Truncate) : File.Create(path);

            if (data != null) {
                var info = new UTF8Encoding(true).GetBytes(data);
                stream.Write(info, 0, info.Length);
            }

            stream.Close();
        }

        /// <summary>
        ///     Deletes the folder.
        /// </summary>
        /// <param name="path">The path.</param>
        private void DeleteFolder(string path) {
            path = QualifyPath(path);

            if (Directory.Exists(path)) {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        ///     Creates the folder.
        /// </summary>
        /// <param name="path">The path.</param>
        private void CreateFolder(string path) {
            path = QualifyPath(path);

            if (!Directory.Exists(path) && path != string.Empty) {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        ///     Reads the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private string ReadFile(string path) {
            path = QualifyPath(path);

            return File.ReadAllText(path);
        }

        /// <summary>
        ///     Opens the file stream.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private FileStream OpenFileStream(string path) {
            path = QualifyPath(path);

            return File.OpenRead(path);
        }

        /// <summary>
        ///     Checks if the file exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private bool FileExists(string path) {
            path = QualifyPath(path);

            return File.Exists(path);
        }

        /// <summary>
        ///     Checks if the folder exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private bool FolderExists(string path) {
            path = QualifyPath(path);

            return Directory.Exists(path);
        }

        public void Dispose() { }
    }
}
