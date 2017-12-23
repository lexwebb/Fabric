using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Data
{
    public abstract class DataPage
    {
        public string Name { get; internal set; }

        protected DataPage(string name) {
            Name = name;
        }
    }
}
