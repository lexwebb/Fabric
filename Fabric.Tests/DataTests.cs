using System;
using System.Collections.Generic;
using System.Text;
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

        private JsonSerializerSettings _serializerSettings => new JsonSerializerSettings() {
            Converters = new List<JsonConverter>() {
                new DataPageSerializer()
            }
        };

        private readonly string _basicRootPageJson = "{\"RootPages\":[\"TestItem1\",\"TestItem2\"],\"Name\":\"root\"}";
        private readonly RootPage<TestRootPage> _basicRootPage =
            new RootPage<TestRootPage> {
                RootPages = new ChildPageCollection<TestRootPage> {
                    new TestRootPage("TestItem1"),
                    new TestRootPage("TestItem2")
                }
            };

        [Fact]
        public void RootPage_ShouldSerialise_Correctly() {
            
            var json = JsonConvert.SerializeObject(_basicRootPage, _serializerSettings);

            Assert.Equal(_basicRootPageJson, json);
        }

        [Fact]
        public void RootPage_ShouldDeserialise_Correctly() {
            var rootPage = JsonConvert.DeserializeObject<RootPage<TestRootPage>>(_basicRootPageJson, _serializerSettings);

            Assert.Equal(_basicRootPage.Name, rootPage.Name);
            Assert.Equal(_basicRootPage.RootPages.GetNames(), rootPage.RootPages.GetNames());
        }
    }
}
