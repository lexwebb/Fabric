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
        public const string DatabaseName = "FabricDatabase";
        public const string RootPageName = "root";

        public FabricDatabase(string databaseRoot, IDatabaseHelper databaseHelper = null,
            IChangeSetHelper changeSetHelper = null, ISchemaManager schemaManager = null,
            IVersionHelper versionHelper = null) {
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

            Resolver.RegisterInstanceOrDefault<IDatabaseHelper, FileSystemDatabaseHelper>(databaseHelper);
            Resolver.RegisterInstanceOrDefault<IChangeSetHelper, ChangeSetHelper>(changeSetHelper);
            Resolver.RegisterInstanceOrDefault<ISchemaManager, SchemaManager>(schemaManager);
            Resolver.RegisterInstanceOrDefault<IVersionHelper, VersionHelper>(versionHelper);
        }

        public UnityContainer Resolver { get; }

        public JsonSerializerSettings SerializerSettings { get; }

        public string DatabaseRoot { get; }

        public string FullDataBaseRoot { get; }

        public Action<FabricDatabase> SeedDatabase { get; set; }

        public DataPage Root { get; private set; }

        /// <summary>
        ///     Initialises this instance.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void Initialise() {
            Global.Instance.Logger.Info("Initialising database");

            var databaseHelper = Resolver.Resolve<IDatabaseHelper>();

            if (databaseHelper.Initialise()) {
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

            var databaseHelper = Resolver.Resolve<IDatabaseHelper>();
            var changeSetHelper = Resolver.Resolve<IChangeSetHelper>();

            Root = new DataPage(RootPageName) {
                ModifiedTimestamp = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds()),
                Parent = new DataPageCollection(null, changeSetHelper, databaseHelper)
            };

            Root.Children = new DataPageCollection(Root, changeSetHelper, databaseHelper);

            databaseHelper.WritePage(Root);
        }

        /// <summary>
        ///     Loads the database.
        /// </summary>
        private void LoadDatabase() {
            Global.Instance.Logger.Info("Loading database file");

            var databaseHelper = Resolver.Resolve<IDatabaseHelper>();
            var changeSetHelper = Resolver.Resolve<IChangeSetHelper>();

            Root = databaseHelper.ReadPage(null);
            Root.Parent = new DataPageCollection(null, changeSetHelper, databaseHelper);
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
