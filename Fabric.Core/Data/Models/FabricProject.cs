using System.Collections.Generic;

namespace Fabric.Core.Data.Models
{
    public class FabricProject : IModel
    {
        public string Name { get; set; }

        public Dictionary<string, bool> Environments { get; set; }
    }
}
