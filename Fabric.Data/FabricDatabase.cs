using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fabric.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Fabric.Data {
    public class FabricDatabase {
        public FabricDatabase(string databaseRoot) {
            DatabaseRoot = databaseRoot;

            if (Path.IsPathRooted(DatabaseRoot)) {
                FullDataBaseRoot = DatabaseRoot;
            }
            else {
                var relativePathRoot = AppDomain.CurrentDomain.BaseDirectory;
                var relativePath = Path.Combine(relativePathRoot, DatabaseRoot);
                FullDataBaseRoot = relativePath;
            }

            SerializerSettings = new JsonSerializerSettings {
                Converters = new List<JsonConverter> {
                    new DataPageSerializer(this)
                },
                Formatting = Formatting.Indented,
                ContractResolver =
                    new CamelCasePropertyNamesContractResolver()
            };
        }

        public JsonSerializerSettings SerializerSettings { get; }

        public string DatabaseRoot { get; }

        public string FullDataBaseRoot { get; }

        public string DatabaseFile => Path.Combine(FullDataBaseRoot, "FabricDatabase.json");

        public Action<FabricDatabase> SeedDatabase { get; set; }

        public RootPage Root { get; private set; }

        public ISchemaManager SchemaManager { get; private set; }

        internal List<ChangeSet> Changes { get; } = new List<ChangeSet>();

        /// <summary>
        ///     Initialises this instance.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void Initialise() {
            Global.Instance.Logger.Info("Initialising database");

            if (!Directory.Exists(FullDataBaseRoot)) Directory.CreateDirectory(FullDataBaseRoot);

            if (!File.Exists(DatabaseFile)) {
                Global.Instance.Logger.Info("Database not found, performing first time setup");
                CreateDatabase();
                InternalSeedDatabase();
            }
            else {
                LoadDatabase();
            }

            SchemaManager = new SchemaManager(this);
            SchemaManager.LoadSchemas();

            Global.Instance.Logger.Info("Finished initialising database");
        }

        /// <summary>
        ///     Creates the database.
        /// </summary>
        private void CreateDatabase() {
            Global.Instance.Logger.Info("Creating database file");
            File.Create(DatabaseFile).Close();

            var settings = new JsonSerializerSettings {
                Converters = new List<JsonConverter> {
                    new DataPageSerializer(this)
                },
                Formatting = Formatting.Indented
            };

            Root = new RootPage(this) {
                ModifiedTimestamp = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds())
            };

            Root.Children = new DataPageCollection(this, Root);

            var json = JsonConvert.SerializeObject(Root, settings);

            File.WriteAllText(DatabaseFile, json);
        }

        private void LoadDatabase() {
            Global.Instance.Logger.Info("Loading database file");

            var settings = new JsonSerializerSettings {
                Converters = new List<JsonConverter> {
                    new DataPageSerializer(this)
                },
                Formatting = Formatting.Indented
            };

            Root = JsonConvert.DeserializeObject<RootPage>(File.ReadAllText(DatabaseFile), settings);
        }

        /// <summary>
        ///     Seeds the database.
        /// </summary>
        private void InternalSeedDatabase() {
            SeedDatabase?.Invoke(this);
        }

        internal List<DataPage> Load(DataPage parent) {
            var parts = FindParentsRecursive(Root, new List<string>());

            return new List<DataPage>();
        }

        private List<string> FindParentsRecursive(DataPage parent, List<string> partsList) {
            while (true) {
                if (parent?.Parent == null) return partsList;
                partsList.Add(parent.Name);
                parent = parent.Parent.Parent;
            }
        }

        public void SaveChanges() {
            var changesTimestamp = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());

            foreach (var changeSet in Changes.Where(c => c.ChangedPage != null)) {
                var parts = FindParentsRecursive(changeSet.ChangedPage, new List<string>());
                switch (changeSet.ChangeType) {
                    case ChangeType.Update:

                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Invalid changeset type: {changeSet.ChangeType} for {nameof(DataPage)}");
                }
            }

            foreach (var changeSet in Changes.Where(c => c.ChangedCollection != null)) {
                var parts = FindParentsRecursive(changeSet.ChangedCollection.Parent, new List<string>());
                parts.Reverse();

                if (parts[0] == "root")
                    parts = parts.Skip(1).ToList();

                var path = parts.Prepend(FullDataBaseRoot).ToArray();

                switch (changeSet.ChangeType) {
                    case ChangeType.Insert:
                        foreach (var dataPageGroup in changeSet.ChangedCollection.GroupBy(d => d.SchemaName)) {
                            var folderPath = Path.Combine(path.Append(dataPageGroup.Key).ToArray());

                            if (!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);

                            foreach (var dataPage in dataPageGroup) {
                                var pageFolderPath =
                                    Path.Combine(path.Append(dataPageGroup.Key).Append(dataPage.Name).ToArray());

                                if (!Directory.Exists(pageFolderPath))
                                    Directory.CreateDirectory(pageFolderPath);

                                var filePath = Path.Combine(pageFolderPath, "dataPage.json");

                                if (!File.Exists(filePath))
                                    File.Create(filePath).Close();

                                dataPage.ModifiedTimestamp = changesTimestamp;

                                File.WriteAllText(filePath, JsonConvert.SerializeObject(dataPage, SerializerSettings));
                            }
                        }
                        break;
                    case ChangeType.Delete:
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Invalid changeset type: {changeSet.ChangeType} for {nameof(DataPageCollection)}");
                }

                var collectionRootFilePath = Path.Combine(path.Append("dataPage.json").ToArray());

                if (changeSet.ChangedCollection.Parent is RootPage)
                    collectionRootFilePath = Path.Combine(path.Append("FabricDatabase.json").ToArray());

                File.WriteAllText(collectionRootFilePath,
                    JsonConvert.SerializeObject(changeSet.ChangedCollection.Parent, SerializerSettings));
            }
        }

        internal void AddChange(ChangeSet changeSet) {
            Changes.Add(changeSet);
        }

        public bool IsPathCollection(string path) {
            path = path?.TrimEnd('/');
            if (string.IsNullOrEmpty(path))
                return false;
            return path.Split('/').Length % 2 != 0;
        }

        public async Task<DataPage> FindChildPage(string path) {
            path = path?.TrimEnd('/');
            try {
                if (IsPathCollection(path))
                    throw new ArgumentException("Provided path points to a collection not a page.");
                return await Task.Run(() => FindChildPageReccursive(Root, path));
            }
            catch (ItemNotFoundException ex) {
                throw new ItemNotFoundException(ex.ItemName, path);
            }
        }

        internal DataPage FindChildPageReccursive(DataPage root, string path) {
            path = path?.TrimEnd('/');
            if (path == null || path.Equals(string.Empty)) return Root;

            var pathParts = path.Split('/');
            var schemaName = pathParts[0];
            var itemName = pathParts[1];

            if (root.Children.Any(c => c.Name == itemName)) {
                var currentPage = root.Children.First(c => c.SchemaName == schemaName && c.Name == itemName);
                return pathParts.Length > 2
                    ? FindChildPageReccursive(currentPage, string.Join("/", pathParts.Skip(2)))
                    : currentPage;
            }

            throw new ItemNotFoundException(itemName);
        }

        public async Task<IEnumerable<DataPage>> FindChildCollection(string path) {
            path = path?.TrimEnd('/');
            try {
                if (!IsPathCollection(path))
                    throw new ArgumentException("Provided path points to a page not a collection.");
                return await Task.Run(() => FindChildCollectionReccursive(Root, path));
            }
            catch (ItemNotFoundException ex) {
                throw new ItemNotFoundException(ex.ItemName, path);
            }
        }

        internal IEnumerable<DataPage> FindChildCollectionReccursive(DataPage root, string path) {
            path = path?.TrimEnd('/');
            if (path == null || path.Equals(string.Empty)) throw new ItemNotFoundException(path);
            ;

            var pathParts = path.Split('/');
            var currentPathRoot = pathParts[0];

            if (root.Children.Any(c => c.SchemaName == currentPathRoot)) {
                if (pathParts.Length == 1) return root.Children.Where(c => c.SchemaName == currentPathRoot);

                var nextCollectionName = pathParts[2];
                var itemName = pathParts[1];

                if (root.Children.Any(c => c.SchemaName == currentPathRoot && c.Name == itemName)) {
                    var item = root.Children.First(c => c.SchemaName == currentPathRoot && c.Name == itemName);
                    return FindChildCollectionReccursive(item, nextCollectionName);
                }
            }

            throw new ItemNotFoundException(currentPathRoot);
        }
    }
}
