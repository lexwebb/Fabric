using System.Collections.Generic;
using Fabric.Core.Models;
using Fabric.Data;

namespace Fabric.Core {
    public class FabricStore : IFabricStore {
        public FabricOptions FabricOptions { get; }

        public FabricDatabase Database { get; private set; }

        public FabricStore(FabricOptions options) {
            this.FabricOptions = options;

            Database = new FabricDatabase(FabricOptions.DataFolderName);
        }

        public IEnumerable<FabricProject> Projects => Database.Root.GetChildren<FabricProject>();
    }
}
