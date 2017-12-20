using System;
using System.Collections.Generic;

namespace Fabric.Core.Data.Models
{
    public class FabricProject : IModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<FabricEnvironment> Environments { get; set; } = new List<FabricEnvironment>();
    }
}
