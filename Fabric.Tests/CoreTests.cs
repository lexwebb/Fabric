using System;
using Xunit;

namespace Fabric.Tests
{
    public class CoreTests
    {
        [Fact]
        public void CoreInitialise_Throws_NotImplementedException() {
            var core = Core.Instance;

            Assert.Throws<NotImplementedException>(() => core.Initialise());
        }
    }
}
