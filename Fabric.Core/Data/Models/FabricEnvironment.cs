namespace Fabric.Core.Data.Models
{
    public class FabricEnvironment : IModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Production { get; set; }
    }
}
