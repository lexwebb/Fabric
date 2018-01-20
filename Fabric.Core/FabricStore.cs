using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Fabric.Core.DebugResources;
using Fabric.Data;

namespace Fabric.Core {
    public class FabricStore : IFabricStore {
        public FabricStore(FabricOptions options) {
            FabricOptions = options;

            Database = new FabricDatabase(FabricOptions.DataFolderName);

#if DEBUG
            var directoryInfo = new DirectoryInfo(Database.FullDataBaseRoot);

            if (directoryInfo.Exists) {
                foreach (var file in directoryInfo.GetFiles()) file.Delete();
                foreach (var dir in directoryInfo.GetDirectories()) dir.Delete(true);
            }
#endif

            Database.Initialise();
            
#if DEBUG
            DebugDataSeeder.SeedDebugData(Database);
#endif
        }

        public FabricOptions FabricOptions { get; }

        public FabricDatabase Database { get; }

        public bool IsPathCollection(string path) {
            return Database.IsPathCollection(path);
        }

        public async Task<DataPage> GetDataPage(string path) {
            return await Database.FindChildPage(path);
        }

        public async Task<IEnumerable<DataPage>> GetDataPageCollection(string path) {
            return await Database.FindChildCollection(path);
        }
    }
}
