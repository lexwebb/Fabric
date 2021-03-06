﻿using System.Collections.Generic;
using System.IO;
using Fabric.Data;
using Newtonsoft.Json;
using Xunit;

namespace Fabric.Tests {
    public class IntegrationTests {
        private JsonSerializerSettings _basicSerializerSettings;

        private readonly string _basicRootPageJson =
            "{\"Name\":\"root\",\"SchemaName\":null,\"PageData\":null,\"Children\":{\"none\":[\"TestItem1\",\"TestItem2\"]}}";

        private FabricDatabase CreateTestingDb(bool deleteOld = true) {
            var databaseDir = Path.Combine(Directory.GetCurrentDirectory(), "TestingDB");

            if (deleteOld) {
                if (Directory.Exists(databaseDir)) {
                    Directory.Delete(databaseDir, true);
                }

                Directory.CreateDirectory(databaseDir);
            }

            var database = new FabricDatabase(databaseDir);
            database.Initialise();

            _basicSerializerSettings = new JsonSerializerSettings {
                Converters = new List<JsonConverter> {
                    new DataPageSerializer(database.Resolver)
                }
            };

            return database;
        }

        [Fact]
        public void RootPage_ShouldSerialise_Correctly() {
            var database = CreateTestingDb();

            database.Root.AddChild("TestItem1", "none", "{}");
            database.Root.AddChild("TestItem2", "none", "{}");

            var json = JsonConvert.SerializeObject(database.Root, _basicSerializerSettings);

            var actual = JsonUtils.Uglify(JsonUtils.RemoveProperty(json, "modifiedTimestamp", true));
            var expected = JsonUtils.Uglify(JsonUtils.RemoveProperty(_basicRootPageJson, "modifiedTimestamp", true));

            Assert.Equal(expected, actual, true);
        }

        [Fact]
        public void RootPageChild_ShouldAdd_Correctly() {
            var database = CreateTestingDb();

            database.Root.AddChild("TestItem1", "none", JsonConvert.SerializeObject(new { testProperty1 = 1, testProperty2 = "foo" }));
            database.Root.AddChild("TestItem2", "none", JsonConvert.SerializeObject(new { testProperty1 = 2, testProperty2 = "bar" }));

            Assert.True(File.Exists(Path.Combine(database.DatabaseRoot, "none", "TestItem1", "dataPage.json")));
            Assert.True(File.Exists(Path.Combine(database.DatabaseRoot, "none", "TestItem2", "dataPage.json")));
        }

        [Fact]
        public void RootPageChild_ShouldDelete_Correctly() {
            var database = CreateTestingDb();

            database.Root.AddChild("TestItem1", "none", JsonConvert.SerializeObject(new { testProperty1 = 1, testProperty2 = "foo" }));
            database.Root.AddChild("TestItem2", "none", JsonConvert.SerializeObject(new { testProperty1 = 2, testProperty2 = "bar" }));

            Assert.True(File.Exists(Path.Combine(database.DatabaseRoot, "none", "TestItem1", "dataPage.json")));
            Assert.True(File.Exists(Path.Combine(database.DatabaseRoot, "none", "TestItem2", "dataPage.json")));

            database.Root.DeleteChild("TestItem1", "none");
            database.Root.GetChild("TestItem2").Delete();

            Assert.False(File.Exists(Path.Combine(database.DatabaseRoot, "none", "TestItem1", "dataPage.json")));
            Assert.False(File.Exists(Path.Combine(database.DatabaseRoot, "none", "TestItem2", "dataPage.json")));
        }

        [Fact]
        public void RootPageChild_ShouldLoad_Correctly() {
            var database = CreateTestingDb();

            database.Root.AddChild("TestItem1", "none", JsonConvert.SerializeObject(new { testProperty1 = 1, testProperty2 = "foo" }));
            database.Root.AddChild("TestItem2", "none", JsonConvert.SerializeObject(new { testProperty1 = 2, testProperty2 = "bar" }));

            Assert.True(File.Exists(Path.Combine(database.DatabaseRoot, "none", "TestItem1", "dataPage.json")));
            Assert.True(File.Exists(Path.Combine(database.DatabaseRoot, "none", "TestItem2", "dataPage.json")));

            // clear database reference
            database = null;
            database = CreateTestingDb(false);

            var item = database.Root.GetChild("TestItem1");
        }

        [Fact]
        public void RootPageChild_ShouldUpdateCorrectly() {
            var database = CreateTestingDb();

            database.Root.AddChild("TestItem1", "none", JsonConvert.SerializeObject(new { testProperty1 = 1, testProperty2 = "foo" }));

            Assert.True(File.Exists(Path.Combine(database.DatabaseRoot, "none", "TestItem1", "dataPage.json")));

            var child = database.Root.GetChild("TestItem1");

            Assert.Equal(JsonConvert.SerializeObject(new { testProperty1 = 1, testProperty2 = "foo" }), child.PageData);

            var data = JsonConvert.SerializeObject(new {test = "TestObject"});
            child.PageData = data;
            child.SaveChanges();

            var text = File.ReadAllText(Path.Combine(database.DatabaseRoot, "none", "TestItem1", "dataPage.json"));
            var node = JsonConvert.DeserializeObject<DataPage>(text, database.SerializerSettings);

            var actual = JsonUtils.Uglify(JsonUtils.RemoveProperty(node.PageData, "modifiedTimestamp", true));
            var expected = JsonUtils.Uglify(JsonUtils.RemoveProperty(data, "modifiedTimestamp", true));

            Assert.Equal(expected, actual, true);
        }
    }
}
