using System.IO;
using Couchbase.Lite.Support;
using Fabric.Data;
using Unity;

namespace Fabric.Tests {
    public class CouchbaseIntegrationTests : IntegrationTestsBase {
        public CouchbaseIntegrationTests() {
            NetDesktop.Activate();
        }

        public override FabricDatabase CreateTestingDbImpl(string databaseDir, bool deleteOld = true) {
            if (deleteOld) {
                if (Directory.Exists(databaseDir)) {
                    Directory.Delete(databaseDir, true);
                }

                Directory.CreateDirectory(databaseDir);
            }

            return new FabricDatabase(databaseDir)
                .UseCouchbase();
        }

        protected override bool CheckPageExistsForPath(string path, FabricDatabase database) {
            var couchbase = database.Resolver.Resolve<IDatabaseHelper>() as CouchbaseDatabaseHelper;
            var name = couchbase.ConvertPagePathToSaveName(path);

            var doc = couchbase.CouchbaseDatabase.GetDocument(name);

            return doc != null;
        }

        protected override string ReadPageRaw(string path, FabricDatabase database) {
            var couchbase = database.Resolver.Resolve<IDatabaseHelper>() as CouchbaseDatabaseHelper;
            var name = couchbase.ConvertPagePathToSaveName(path);

            var doc = couchbase.CouchbaseDatabase.GetDocument(name);

            return doc.GetString(CouchbaseDatabaseHelper.DataObjectName);
        }
    }
}
