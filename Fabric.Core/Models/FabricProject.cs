using Fabric.Data;

namespace Fabric.Core.Models
{
    public class FabricProject : DataPage
    {
        public FabricProject() : base("Default") { }
        public FabricProject(string name) : base(name) { }
    }
}
