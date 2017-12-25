using Fabric.Data;

namespace Fabric.Core.Data.Models
{
    public class FabricEnvironment : DataPage
    {
        public bool Production { get; set; }

        public FabricEnvironment(string name) : base(name) { }
    }
}
