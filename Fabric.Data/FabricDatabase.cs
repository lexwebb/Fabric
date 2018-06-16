using System;
using System.Collections.Generic;
using System.IO;
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

        public FabricDatabase(string databaseRoot, IDataWriter dataWriter = null, IDataReader dataReader = null,
            IChangeSetHelper changeSetHelper = null, ISchemaManager schemaManager = null) {
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
                    new DataPageSerializer(Resolver)
                },
                Formatting = Formatting.Indented,
                ContractResolver =
                    new CamelCasePropertyNamesContractResolver()
            };

            Resolver.RegisterInstance(SerializerSettings);

            if (dataWriter != null) {
                Resolver.RegisterInstance(dataWriter);
            }
            else {
                Resolver.RegisterSingleton<IDataWriter, DataWriter>();
            }

            if (dataReader != null) {
                Resolver.RegisterInstance(dataReader);
            }
            else {
                Resolver.RegisterSingleton<IDataReader, DataReader>();
            }

            if (changeSetHelper != null) {
                Resolver.RegisterInstance(changeSetHelper);
            }
            else {
                Resolver.RegisterSingleton<IChangeSetHelper, ChangeSetHelper>();
            }

            if (schemaManager != null) {
                Resolver.RegisterInstance(schemaManager);
            }
            else {
                Resolver.RegisterSingleton<ISchemaManager, SchemaManager>();
            }
        }

        public UnityContainer Resolver { get; }

        public JsonSerializerSettings SerializerSettings { get; }

        public string DatabaseRoot { get; }

        public string FullDataBaseRoot { get; }

        public string DatabaseFilePath => Path.Combine(FullDataBaseRoot, DatabaseFileName);

        public Action<FabricDatabase> SeedDatabase { get; set; }

        public DataPage Root { get; private set; }

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
            var reader = Resolver.Resolve<IDataReader>();
            var changeSetHelper = Resolver.Resolve<IChangeSetHelper>();

            writer.WriteFile(DatabaseFilePath);

            Root = new DataPage(RootPageName) {
                ModifiedTimestamp = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds()),
                Parent = new DataPageCollection(null, changeSetHelper, reader)
            };

            Root.Children = new DataPageCollection(Root, changeSetHelper, reader);

            writer.WritePage(Root);
        }

        /// <summary>
        ///     Loads the database.
        /// </summary>
        private void LoadDatabase() {
            Global.Instance.Logger.Info("Loading database file");

            var reader = Resolver.Resolve<IDataReader>();
            var changeSetHelper = Resolver.Resolve<IChangeSetHelper>();

            Root = reader.ReadPage(DatabaseFilePath);
            Root.Parent = new DataPageCollection(null, changeSetHelper, reader);
        }

        /// <summary>
        ///     Seeds the database.
        /// </summary>
        private void InternalSeedDatabase() {
            SeedDatabase?.Invoke(this);
        }

        /// <summary>
        ///     Determines whether [is path collection] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     <c>true</c> if [is path collection] [the specified path]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPathCollection(string path) {
            return Utils.IsPathCollection(path);
        }

        /// <summary>
        ///     Finds the child page.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Task<DataPage> FindChildPage(string path) {
            return Utils.FindChildPage(Root, path);
        }

        /// <summary>
        ///     Finds the child collection.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Task<IEnumerable<DataPage>> FindChildCollection(string path) {
            return Utils.FindChildCollection(Root, path);
        }
    }
}
