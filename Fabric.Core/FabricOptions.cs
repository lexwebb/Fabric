using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Core
{
    public class FabricOptions {
        public string DataFolderName { get; set; } = "FabricStore";

        public static FabricOptions Deafult() {
            return new FabricOptions();
        }
    }
}
