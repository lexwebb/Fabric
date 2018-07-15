using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fabric.Data;

namespace Fabric.Tests
{
    public class FileSystemIntegrationTests : IntegrationTestsBase
    {
        public override FabricDatabase CreateTestingDbImpl(bool deleteOld = true) {
            var databaseDir = Path.Combine(Directory.GetCurrentDirectory(), "TestingDB");

            if (deleteOld) {
                if (Directory.Exists(databaseDir)) {
                    Directory.Delete(databaseDir, true);
                }

                Directory.CreateDirectory(databaseDir);
            }

            return new FabricDatabase(databaseDir);
        }

        protected override bool CheckPageExistsForPath(string path, FabricDatabase database) {
            return File.Exists(Path.Combine(database.DatabaseRoot, path, "dataPage.json"));
        }

        protected override string ReadPageRaw(string path, FabricDatabase database) {
            return File.ReadAllText(Path.Combine(database.DatabaseRoot, path, "dataPage.json"));
        }
    }
}
