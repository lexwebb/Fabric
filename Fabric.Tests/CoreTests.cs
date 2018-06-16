using System.IO;
using System.Reflection;
using Fabric.Data;
using Fabric.Logging;
using Xunit;

namespace Fabric.Tests {
    public class CoreTests {
        private static string GetFileContents(string sampleFile) {
            var asm = Assembly.GetExecutingAssembly();
            var resource = $"Fabric.Tests.TestAssets.{sampleFile}";
            using (var stream = asm.GetManifestResourceStream(resource)) {
                if (stream == null) {
                    return string.Empty;
                }

                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        [Fact]
        public void CoreInitialise_InitialisesDefaults_Correctly() {
            Assert.NotNull(Global.Instance);

            var core = Global.Instance;

            Assert.NotNull(core.Logger);
            Assert.IsType<ConsoleGlobalLogger>(core.Logger);
        }

        [Fact]
        public void JsonPropertyRemover_RemovesProperty_Correctly() {
            var input = GetFileContents("RecursivePropertyRemove_Input.json");
            var expected = GetFileContents("RecursivePropertyRemove_Result.json");
            var actual = JsonUtils.RemoveProperty(input, "ModifiedTimestamp", true);

            Assert.Equal(JsonUtils.Uglify(expected), JsonUtils.Uglify(actual), true);
        }
    }
}
