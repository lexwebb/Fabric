using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Fabric.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Fabric.Tests
{
    public class DataTests
    {
        private static string GetFileContents(string sampleFile) {
            var asm = Assembly.GetExecutingAssembly();
            var resource = $"Fabric.Tests.TestAssets.{sampleFile}";
            using (var stream = asm.GetManifestResourceStream(resource)) {
                if (stream == null) return string.Empty;
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        public class TestRootPage : DataPage {
            public string TestProperty = "Banana";

            public TestRootPage(string name) : base(name) { }
        }

        public class TestSubPage : DataPage {
            public TestSubPage(string name) : base(name) { }
        }

        private JsonSerializerSettings _basicSerializerSettings;

        private readonly string _basicRootPageJson = "{\"Name\":\"root\",\"ModifiedTimestamp\":\"20180105220218519\",\"Children\":{\"TestRootPage\":[\"TestItem1\",\"TestItem2\"]}}";

        private FabricDatabase CreateTestingDb() {
            var databaseDir = Path.Combine(Directory.GetCurrentDirectory(), "TestingDB");

            if(Directory.Exists(databaseDir))
                Directory.Delete(databaseDir, true);

            Directory.CreateDirectory(databaseDir);

            var database = new FabricDatabase(databaseDir);

            _basicSerializerSettings = new JsonSerializerSettings() {
                Converters = new List<JsonConverter>() {
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

            Assert.Equal(JsonUtils.Uglify(expected), JsonUtils.Uglify(actual));
        }

        [Fact]
        public void RootPage_ShouldSerialise_Correctly() {
            var database = CreateTestingDb();
            
            database.Root.Children.Add(new TestRootPage("TestItem1"));
            database.Root.Children.Add(new TestRootPage("TestItem2"));

            var json = JsonConvert.SerializeObject(database.Root, _basicSerializerSettings);

            var actual = JsonUtils.RemoveProperty(json, "ModifiedTimestamp", true);
            var expected = JsonUtils.RemoveProperty(_basicRootPageJson, "ModifiedTimestamp", true);

            Assert.Equal(JsonUtils.Uglify(expected), JsonUtils.Uglify(actual));
        }

        [Fact]
        public void RootPageChild_ShouldAdd_Correctly() {
            var database = CreateTestingDb();

            database.Root.Children.Add(new TestRootPage("TestItem1"));
            database.Root.Children.Add(new TestRootPage("TestItem2"));
            database.SaveChanges();

            Assert.True(File.Exists(Path.Combine(database.DatabaseRoot, "TestRootPage", "TestItem1", "dataPage.json")));
            Assert.True(File.Exists(Path.Combine(database.DatabaseRoot, "TestRootPage", "TestItem2", "dataPage.json")));
        }
    }
}
