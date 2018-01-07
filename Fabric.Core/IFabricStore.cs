using System;
using System.Collections.Generic;
using System.Text;
using Fabric.Core.Models;
using Fabric.Data;

namespace Fabric.Core
{
    public interface IFabricStore
    {
        FabricOptions FabricOptions { get; }

        FabricDatabase Database { get; }

        IEnumerable<FabricProject> Projects { get; }
    }
}
