using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fabric.Logging;
using Newtonsoft.Json;

namespace Fabric.Data
{
    public class FabricDatabase<T> where T : DataPage
    {
        public string DatabaseRoot { get; }

        public string DatabaseFile => Path.Combine(DatabaseRoot, "FabricDatabase.json");

        public Type RootPageType { get; private set; }

        public FabricDatabase(string databaseRoot) {
            this.DatabaseRoot = databaseRoot;
            Initialise();
        }

        private void Initialise() {
            Global.Instance.Logger.Info("Initialising database");

            if (!Directory.Exists(Path.GetDirectoryName(DatabaseRoot))) {
                var error = $"Could not find directory scedified to store DB: {DatabaseRoot}";
                Global.Instance.Logger.Error(error);
                throw new DirectoryNotFoundException(error);
            }

            RootPageType = typeof(T);

            if (!File.Exists(DatabaseFile)) {
                Global.Instance.Logger.Info("Database not found, performing first time setup");
                CreateDatabase();
                SeedDatabase();
            }

            Global.Instance.Logger.Info("Finished initialising database");
        }

        private void CreateDatabase() {
            Global.Instance.Logger.Info("Creating database file");
            File.Create(DatabaseFile);

            var settings = new JsonSerializerSettings() {
                Converters = new List<JsonConverter>() {
                    new DataPageSerializer()
                }
            };
            //var json = JsonConvert.SerializeObject(new RootPage(), settings);
        }

        private void SeedDatabase() {
            throw new NotImplementedException();
        }

        
    }
}
