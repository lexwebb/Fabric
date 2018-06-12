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
        public FabricDatabase(string databaseRoot, IChangeSetHelper changeSetHelper = null) {
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

            ChangeSetHelper = changeSetHelper ?? new ChangeSetHelper();
        }

        public JsonSerializerSettings SerializerSettings { get; }

        public string DatabaseRoot { get; }

        public string FullDataBaseRoot { get; }

        public string DatabaseFile => Path.Combine(FullDataBaseRoot, "FabricDatabase.json");

        public Action<FabricDatabase> SeedDatabase { get; set; }

        public DataPage Root { get; private set; }

        public ISchemaManager SchemaManager { get; private set; }

        internal IChangeSetHelper ChangeSetHelper { get; set; }

        internal List<ChangeSet> Changes { get; } = new List<ChangeSet>();

        /// <summary>
        ///     Initialises this instance.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void Initialise() {
            Global.Instance.Logger.Info("Initialising database");

            if (!Directory.Exists(FullDataBaseRoot)) {
                Directory.CreateDirectory(FullDataBaseRoot);
            }

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

            Root = new DataPage("root") {
                ModifiedTimestamp = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds()),
                Parent = new DataPageCollection(this, null)
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

            Root = JsonConvert.DeserializeObject<DataPage>(File.ReadAllText(DatabaseFile), settings);
            Root.Parent = new DataPageCollection(this, null);
        }

        /// <summary>
        ///     Seeds the database.
        /// </summary>
        private void InternalSeedDatabase() {
            SeedDatabase?.Invoke(this);
        }

        public void SaveChanges() {
            var changesTimestamp = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());

            for (var i = Changes.Count(c => c.ChangedPage != null) - 1; i >= 0; i--) {
                var changeSet = Changes.Where(c => c.ChangedPage != null).ToList()[i];

                var pathParts = Utils.FindParentsRecursive(changeSet.ChangedPage.Parent.Parent, new List<string>());
                pathParts.Reverse();

                if (pathParts[0] == "root") {
                    pathParts = pathParts.Skip(1).ToList();
                }

                var collectionPath = pathParts.Prepend(FullDataBaseRoot).ToArray();

                switch (changeSet.ChangeType) {
                    case ChangeType.Update:
                        ChangeSetHelper.Update(this, changesTimestamp, changeSet);
                        break;
                    case ChangeType.Insert:
                        ChangeSetHelper.Insert(this, changesTimestamp, changeSet, collectionPath);
                        ChangeSetHelper.SaveCollectionChanges(changeSet, collectionPath, this);
                        break;
                    case ChangeType.Delete:
                        ChangeSetHelper.Delete(this, changesTimestamp, changeSet);
                        ChangeSetHelper.SaveCollectionChanges(changeSet, collectionPath, this);
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Invalid changeset type: {changeSet.ChangeType} for {nameof(DataPage)}");
                }

                Changes.RemoveAt(i);
            }
        }

        internal void AddChange(ChangeSet changeSet) {
            Changes.Add(changeSet);
        }

        public bool IsPathCollection(string path) {
            return Utils.IsPathCollection(path);
        }

        public Task<DataPage> FindChildPage(string path) {
            return Utils.FindChildPage(Root, path);
        }

        public Task<IEnumerable<DataPage>> FindChildCollection(string path) {
            return Utils.FindChildCollection(Root, path);
        }

        public DataPage LoadPage(string path) {
            return JsonConvert.DeserializeObject<DataPage>(path, SerializerSettings);
        }
    }
}
