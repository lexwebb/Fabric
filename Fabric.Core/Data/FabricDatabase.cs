using System;
using System.IO;
using Fabric.Core.Data.Models;
using LiteDB;

namespace Fabric.Core.Data {
    public class FabricDatabase {

        public static FabricDatabase Instance { get; internal set; }

        internal Stream DbStream;
        internal string DbFileName;
        internal bool IsStream = false;
        internal bool Initialised = false;

        private FabricDatabase() {

        }

        public static void Initialise(Stream stream) {
            Instance = new FabricDatabase();

            try {
                using (var db = new LiteDatabase(stream)) {
                    Instance.EnsureDatabaseExists(db);
                }
            } catch (Exception ex) {
                Global.Instance.Logger.Error("Error loading internal database", ex);
                throw new Exception("Error loading internal database");
            }

            Instance.DbStream = stream;
            Instance.IsStream = true;
            Instance.Initialised = true;

            Instance.InitialiseRelationships();
        }

        public static void Initialise(string fileName) {
            Instance = new FabricDatabase();

            try {
                using (var db = new LiteDatabase(fileName)) {
                    Instance.EnsureDatabaseExists(db);
                }
            } catch (Exception ex) {
                Global.Instance.Logger.Error("Error loading internal database", ex);
                throw new Exception("Error loading internal database");
            }

            Instance.DbFileName = fileName;
            Instance.IsStream = false;
            Instance.Initialised = true;

            Instance.InitialiseRelationships();
        }

        private void InitialiseRelationships() {
            var mapper = BsonMapper.Global;

            mapper.Entity<FabricProject>()
                .DbRef(x => x.Environments, "environments");
        }

        private void EnsureDatabaseExists(LiteDatabase db) {
            try {
                var engine = db.Engine;
            } catch (Exception ex) {
                Global.Instance.Logger.Error("Error loading internal database", ex);
                throw new Exception("Error loading internal database");
            }
        }

        internal LiteDatabase GetConnection() {
            try {
                if (!Initialised) {
                    throw new InvalidOperationException("FabricDatabase instance must be intilised before accessing");    
                }

                if (IsStream) {
                    using (var db = new LiteDatabase(DbStream)) {
                        return db;
                    }
                }

                using (var db = new LiteDatabase(DbFileName)) {
                    return db;
                }
            } catch (Exception ex) {
                Global.Instance.Logger.Error("Error loading internal database", ex);
                throw new Exception("Error loading internal database");
            }
        }

        public static LiteCollection<FabricProject> Projects => Instance.GetConnection().GetCollection<FabricProject>();

        public static LiteCollection<FabricEnvironment> Environments => Instance.GetConnection().GetCollection<FabricEnvironment>();
    }
}
