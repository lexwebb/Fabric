using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fabric.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Unity;

namespace Fabric.Data {
    public class FabricDatabase {
        public const string DatabaseFileName = "FabricDatabase.json";
        public const string DataPageFileName = "dataPage.json";
        public const string RootPageName = "root";

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

            Resolver = new UnityContainer();
            Resolver.RegisterInstance(this);

            SerializerSettings = new JsonSerializerSettings {
                Converters = new List<JsonConverter> {
                    new DataPageSerializer(this)
                },
                Formatting = Formatting.Indented,
                ContractResolver =
                    new CamelCasePropertyNamesContractResolver()
            };

            Resolver.RegisterInstance(SerializerSettings);
            Resolver.RegisterSingleton<IDataWriter, DataWriter>();
            Resolver.RegisterSingleton<IDataReader, DataReader>();
            Resolver.RegisterSingleton<IChangeSetHelper, ChangeSetHelper>();
            Resolver.RegisterSingleton<ISchemaManager, SchemaManager>();
        }

        public UnityContainer Resolver { get; }

        public JsonSerializerSettings SerializerSettings { get; }

        public string DatabaseRoot { get; }

        public string FullDataBaseRoot { get; }

        public string DatabaseFilePath => Path.Combine(FullDataBaseRoot, DatabaseFileName);

        public Action<FabricDatabase> SeedDatabase { get; set; }

        public DataPage Root { get; private set; }

        internal List<ChangeSet> Changes { get; } = new List<ChangeSet>();

        /// <summary>
        ///     Initialises this instance.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void Initialise() {
            Global.Instance.Logger.Info("Initialising database");

            var reader = Resolver.Resolve<IDataReader>();
            var writer = Resolver.Resolve<IDataWriter>();

            if (!reader.FolderExists(FullDataBaseRoot)) {
                writer.CreateFolder(FullDataBaseRoot);
            }

            if (!reader.FileExists(DatabaseFilePath)) {
                Global.Instance.Logger.Info("Database not found, performing first time setup");
                CreateDatabase();
                InternalSeedDatabase();
            }
            else {
                LoadDatabase();
            }

            Resolver.Resolve<ISchemaManager>().LoadSchemas();

            Global.Instance.Logger.Info("Finished initialising database");
        }

        /// <summary>
        ///     Creates the database.
        /// </summary>
        private void CreateDatabase() {
            Global.Instance.Logger.Info("Creating database file");

            var writer = Resolver.Resolve<IDataWriter>();

            writer.WriteFile(DatabaseFilePath);

            Root = new DataPage(RootPageName) {
                ModifiedTimestamp = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds()),
                Parent = new DataPageCollection(this, null)
            };

            Root.Children = new DataPageCollection(this, Root);

            writer.WritePage(Root);
        }

        private void LoadDatabase() {
            Global.Instance.Logger.Info("Loading database file");

            var reader = Resolver.Resolve<IDataReader>();

            Root = reader.ReadPage(DatabaseFilePath);
            Root.Parent = new DataPageCollection(this, null);
        }

        /// <summary>
        ///     Seeds the database.
        /// </summary>
        private void InternalSeedDatabase() {
            SeedDatabase?.Invoke(this);
        }

        /// <summary>
        ///     Saves the changes.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void SaveChanges() {
            var changesTimestamp = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());
            var changeSetHelper = Resolver.Resolve<IChangeSetHelper>();

            for (var i = Changes.Count(c => c.ChangedPage != null) - 1; i >= 0; i--) {
                var changeSet = Changes.Where(c => c.ChangedPage != null).ToList()[i];

                var pathParts = Utils.FindParentsRecursive(changeSet.ChangedPage.Parent.Parent, new List<string>());
                pathParts.Reverse();

                if (pathParts[0] == RootPageName) {
                    pathParts = pathParts.Skip(1).ToList();
                }

                var collectionPath = pathParts.Prepend(FullDataBaseRoot).ToArray();

                switch (changeSet.ChangeType) {
                    case ChangeType.Update:
                        changeSetHelper.Update(changesTimestamp, changeSet);
                        break;
                    case ChangeType.Insert:
                        changeSetHelper.Insert(changesTimestamp, changeSet, collectionPath);
                        break;
                    case ChangeType.Delete:
                        changeSetHelper.Delete(changesTimestamp, changeSet);
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
            if (!path.StartsWith(DatabaseRoot)) {
                path = Path.Combine(DatabaseRoot, path);
            }

            return Resolver.Resolve<DataReader>().ReadPage(path);
        }
    }
}
