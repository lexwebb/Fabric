using System.Threading.Tasks;
using Fabric.Data;

namespace Fabric.Core {
    public class FabricStore : IFabricStore {
        public FabricOptions FabricOptions { get; }

        public FabricDatabase Database { get; private set; }

        public FabricStore(FabricOptions options) {
            this.FabricOptions = options;

            Database = new FabricDatabase(FabricOptions.DataFolderName);
        }

        public async Task<DataPage> GetDataPage(string path) {
            return await Database.FindChildPage(path);
        }
    }
}
