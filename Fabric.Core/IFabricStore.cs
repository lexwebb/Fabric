using System.Threading.Tasks;
using Fabric.Data;

namespace Fabric.Core
{
    public interface IFabricStore
    {
        FabricOptions FabricOptions { get; }

        FabricDatabase Database { get; }

        Task<DataPage> GetDataPage(string path);
    }
}
