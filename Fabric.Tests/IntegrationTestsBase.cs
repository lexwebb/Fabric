using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Fabric.Data;
using Newtonsoft.Json;
using Xunit;

namespace Fabric.Tests {
    public abstract class IntegrationTestsBase {
        private JsonSerializerSettings _basicSerializerSettings;

        private string DatabaseDir => Path.Combine(Directory.GetCurrentDirectory(), "TestingDB");

        private readonly string _basicRootPageJson =
            "{\"Name\":\"root\",\"SchemaName\":null,\"PageData\":null,\"Children\":{\"none\":[\"TestItem1\",\"TestItem2\"]}}";

        private FabricDatabase CreateTestingDb(bool deleteOld = true) {
            var database = CreateTestingDbImpl(DatabaseDir, deleteOld);

            database.Initialise();

            _basicSerializerSettings = new JsonSerializerSettings {
                Converters = new List<JsonConverter> {
                    new DataPageSerializer(database.Resolver)
                }
            };

            return database;
        }

        public abstract FabricDatabase CreateTestingDbImpl(string databaseDir, bool deleteOld = true);

        protected abstract bool CheckPageExistsForPath(string path, FabricDatabase database);

        protected abstract string ReadPageRaw(string path, FabricDatabase database);

        protected IntegrationTestsBase() {
            var deleted = false;
            var looped = 0;

            while (!deleted) {
                try {
                    Directory.Delete(DatabaseDir, true);
                    break;
                }
                catch (Exception) {
                    looped++;

                    if (looped > 10) {
                        throw new Exception("Testing delete deadlocked");
                    }

                    Task.Delay(100).Wait();
                }
            }
        }

        [Fact]
        public void RootPage_ShouldSerialise_Correctly() {
            using (var database = CreateTestingDb()) {
                database.Root.AddChild("TestItem1", "none", "{}");
                database.Root.AddChild("TestItem2", "none", "{}");

                var json = JsonConvert.SerializeObject(database.Root, _basicSerializerSettings);

                var actual = JsonUtils.Uglify(JsonUtils.RemoveProperty(json, "modifiedTimestamp", true));
                var expected =
                    JsonUtils.Uglify(JsonUtils.RemoveProperty(_basicRootPageJson, "modifiedTimestamp", true));

                Assert.Equal(expected, actual, true);
            }
        }

        [Fact]
        public void RootPageChild_ShouldAdd_Correctly() {
            using (var database = CreateTestingDb()) {
                database.Root.AddChild("TestItem1", "none",
                    JsonConvert.SerializeObject(new {testProperty1 = 1, testProperty2 = "foo"}));
                database.Root.AddChild("TestItem2", "none",
                    JsonConvert.SerializeObject(new {testProperty1 = 2, testProperty2 = "bar"}));

                Assert.True(CheckPageExistsForPath(Path.Combine("none", "TestItem1"), database));
                Assert.True(CheckPageExistsForPath(Path.Combine("none", "TestItem2"), database));
            }
        }

        [Fact]
        public void RootPageChild_ShouldDelete_Correctly() {
            using (var database = CreateTestingDb()) {
                database.Root.AddChild("TestItem1", "none",
                    JsonConvert.SerializeObject(new {testProperty1 = 1, testProperty2 = "foo"}));
                database.Root.AddChild("TestItem2", "none",
                    JsonConvert.SerializeObject(new {testProperty1 = 2, testProperty2 = "bar"}));

                Assert.True(CheckPageExistsForPath(Path.Combine("none", "TestItem1"), database));
                Assert.True(CheckPageExistsForPath(Path.Combine("none", "TestItem2"), database));

                database.Root.DeleteChild("TestItem1", "none");
                database.Root.GetChild("TestItem2").Delete();

                Assert.False(CheckPageExistsForPath(Path.Combine("none", "TestItem1"), database));
                Assert.False(CheckPageExistsForPath(Path.Combine("none", "TestItem2"), database));
            }
        }

        [Fact]
        public void RootPageChild_ShouldLoad_Correctly() {
            var testItem1Data = JsonConvert.SerializeObject(new {testProperty1 = 1, testProperty2 = "foo"});
            var testItem2Data = JsonConvert.SerializeObject(new {testProperty1 = 2, testProperty2 = "bar"});

            // We create two seperate db's to properly test loading

            using (var database = CreateTestingDb()) {
                database.Root.AddChild("TestItem1", "none", testItem1Data);
                database.Root.AddChild("TestItem2", "none", testItem2Data);

                Assert.True(CheckPageExistsForPath(Path.Combine("none", "TestItem1"), database));
                Assert.True(CheckPageExistsForPath(Path.Combine("none", "TestItem2"), database));
            }

            using (var database = CreateTestingDb(false)) {
                var item1 = database.Root.GetChild("TestItem1");
                var item2 = database.Root.GetChild("TestItem2");

                var item1Actual = JsonUtils.Uglify(JsonUtils.RemoveProperty(item1.PageData, "modifiedTimestamp", true));
                var item1Expected =
                    JsonUtils.Uglify(JsonUtils.RemoveProperty(testItem1Data, "modifiedTimestamp", true));

                var item2Actual = JsonUtils.Uglify(JsonUtils.RemoveProperty(item2.PageData, "modifiedTimestamp", true));
                var item2Expected =
                    JsonUtils.Uglify(JsonUtils.RemoveProperty(testItem2Data, "modifiedTimestamp", true));

                Assert.Equal(item1Expected, item1Actual);
                Assert.Equal(item2Expected, item2Actual);
            }
        }

        [Fact]
        public void RootPageChild_ShouldUpdateCorrectly() {
            using (var database = CreateTestingDb()) {
                database.Root.AddChild("TestItem1", "none",
                    JsonConvert.SerializeObject(new {testProperty1 = 1, testProperty2 = "foo"}));

                Assert.True(CheckPageExistsForPath(Path.Combine("none", "TestItem1"), database));

                var child = database.Root.GetChild("TestItem1");

                Assert.Equal(JsonConvert.SerializeObject(new {testProperty1 = 1, testProperty2 = "foo"}),
                    child.PageData);

                var data = JsonConvert.SerializeObject(new {test = "TestObject"});
                child.PageData = data;
                child.SaveChanges();

                var text = ReadPageRaw(Path.Combine("none", "TestItem1"), database);
                var node = JsonConvert.DeserializeObject<DataPage>(text, database.SerializerSettings);

                var actual = JsonUtils.Uglify(JsonUtils.RemoveProperty(node.PageData, "modifiedTimestamp", true));
                var expected = JsonUtils.Uglify(JsonUtils.RemoveProperty(data, "modifiedTimestamp", true));

                Assert.Equal(expected, actual, true);
            }
        }
    }
}
