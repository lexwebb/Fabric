using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Fabric.Data;
using Newtonsoft.Json;
using Xunit;

namespace Fabric.Tests {
    public class DataTests {
        private static string GetFileContents(string sampleFile) {
            var asm = Assembly.GetExecutingAssembly();
            var resource = $"Fabric.Tests.TestAssets.{sampleFile}";
            using (var stream = asm.GetManifestResourceStream(resource)) {
                if (stream == null) return string.Empty;
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        public class TestSubPage : DataPage {
            public TestSubPage(string name) : base(name) { }
        }

        private JsonSerializerSettings _basicSerializerSettings;

        private readonly string _basicRootPageJson =
                "{\"Name\":\"root\",\"SchemaName\":null,\"PageData\":null,\"Children\":{\"none\":[\"TestItem1\",\"TestItem2\"]}}"
            ;

        private FabricDatabase CreateTestingDb() {
            var databaseDir = Path.Combine(Directory.GetCurrentDirectory(), "TestingDB");

            if (Directory.Exists(databaseDir))
                Directory.Delete(databaseDir, true);

            Directory.CreateDirectory(databaseDir);

            var database = new FabricDatabase(databaseDir);
            database.Initialise();

            _basicSerializerSettings = new JsonSerializerSettings {
                Converters = new List<JsonConverter> {
                    new DataPageSerializer(database)
                }
            };

            return database;
        }

        [Fact]
        public void JsonPropertRemover_RemovesProperty_Correctly() {
            var input = GetFileContents("RecursivePropertyRemove_Input.json");
            var expected = GetFileContents("RecursivePropertyRemove_Result.json");
            var actual = JsonUtils.RemoveProperty(input, "ModifiedTimestamp", true);

            Assert.Equal(JsonUtils.Uglify(expected), JsonUtils.Uglify(actual), true);
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

            database.Root.AddChild("TestItem1", "none", "{}");
            database.Root.AddChild("TestItem2", "none", "{}");
            database.SaveChanges();

            Assert.True(File.Exists(Path.Combine(database.DatabaseRoot, "none", "TestItem1", "dataPage.json")));
            Assert.True(File.Exists(Path.Combine(database.DatabaseRoot, "none", "TestItem2", "dataPage.json")));
        }
    }
}
