using System;
using Fabric.Core;
using Xunit;

namespace Fabric.Tests
{
    public class CoreTests
    {
        [Fact]
        public void CoreInitialise_Throws_NotImplementedException() {
            var core = Global.Instance;

            Assert.Throws<NotImplementedException>(() => core.Initialise());
        }
    }
}
