using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Data
{
    public class RootPage<T> : DataPage where T : DataPage {
        public ChildPageCollection<T> RootPages { get; set; }
        public RootPage() : base("root") { }
    }
}
