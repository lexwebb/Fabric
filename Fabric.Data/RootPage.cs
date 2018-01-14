namespace Fabric.Data
{
    public class RootPage : DataPage {
        public RootPage(FabricDatabase database) : base("root") {
            this.Parent = new DataPageCollection(database, null);
        }
    }
}
