namespace Fabric.Core {
    public class FabricDatabase : IFabricDatabase {
        public FabricOptions FabricOptions { get; }

        public FabricDatabase(FabricOptions options) {
            this.FabricOptions = options;
        }
    }
}
