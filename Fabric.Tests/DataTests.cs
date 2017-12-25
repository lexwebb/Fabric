using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Fabric.Data;
using Newtonsoft.Json;
using Xunit;

namespace Fabric.Tests
{
    public class DataTests
    {
        public class TestRootPage : DataPage {
            public string TestProperty = "Banana";

            public TestRootPage(string name) : base(name) { }
        }

        public class TestSubPage : DataPage {
            public TestSubPage(string name) : base(name) { }
        }

        private JsonSerializerSettings _basicSerializerSettings;

        private readonly string _basicRootPageJson = "{\"RootPages\":[\"TestItem1\",\"TestItem2\"],\"Name\":\"root\"}";

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
        public void RootPage_ShouldSerialise_Correctly() {
            var database = CreateTestingDb();
            
            database.Data.Children.Add(new TestRootPage("TestItem1"));
            database.Data.Children.Add(new TestRootPage("TestItem2"));

            var json = JsonConvert.SerializeObject(database.Data, _basicSerializerSettings);

            Assert.Equal(_basicRootPageJson, json);
        }

        [Fact]
        public void RootPageChild_ShouldAdd_Correctly() {
            var database = CreateTestingDb();

            database.Data.Children.Add(new TestRootPage("TestItem1"));
            database.Data.Children.Add(new TestRootPage("TestItem2"));
            database.SaveChanges();
        }

        //[Fact]
        //public void RootPage_ShouldDeserialise_Correctly() {
        //    var rootPage = JsonConvert.DeserializeObject<RootPage>(_basicRootPageJson, _basicSerializerSettings);

        //    //Assert.Equal(_basicRootPage.Name, rootPage.Name);
        //    //Assert.Equal(_basicRootPage.RootPages.GetNames(), rootPage.RootPages.GetNames());
        //}

        //[Fact]
        //public void FabricDatabase_ShouldBuildFileTree_Correctly() {
        //    var database = CreateTestingDb();

        //    var expectedTree = new TreeNode<Type>(typeof(TestRootPage));
        //    expectedTree.AddChild(typeof(TestSubPage));

        //    //Assert.Equal(expectedTree.Value, database.FileTree.Value);
        //    //Assert.Equal(expectedTree.Children.First().Value, database.FileTree.Children.First().Value);
        //}
    }
}
