namespace Fabric.Core {
    public class FabricStore : IFabricStore {
        public FabricOptions FabricOptions { get; }

        public FabricStore(FabricOptions options) {
            this.FabricOptions = options;
        }
    }
}
