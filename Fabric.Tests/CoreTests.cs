using Fabric.Logging;
using Xunit;

namespace Fabric.Tests {
    public class CoreTests {
        [Fact]
        public void CoreInitialise_InitialisesDefaults_Correctly() {
            Assert.NotNull(Global.Instance);

            var core = Global.Instance;

            Assert.NotNull(core.Logger);
            Assert.IsType<ConsoleGlobalLogger>(core.Logger);
        }
    }
}
