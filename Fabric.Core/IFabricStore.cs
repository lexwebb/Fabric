using System.Collections.Generic;
using System.Threading.Tasks;
using Fabric.Data;

namespace Fabric.Core {
    public interface IFabricStore {
        FabricOptions FabricOptions { get; }

        FabricDatabase Database { get; }

        bool IsPathCollection(string path);

        Task<DataPage> GetDataPage(string path);

        Task<IEnumerable<DataPage>> GetDataPageCollection(string path);
    }
}
