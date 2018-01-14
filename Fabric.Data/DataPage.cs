using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fabric.Data
{
    public class DataPage {
        public string Name { get; internal set; }

        public string ModifiedTimestamp { get; internal set; }

        public string SchemaName { get; internal set; }

        public string PageData { get; set; }

        public DataPageCollection Children { get; set; }

        public IEnumerable<T> GetChildren<T>() where T : DataPage {
            return Children.OfType<T>();
        }

        internal DataPageCollection Parent { get; set; }

        internal bool Dirty { get; set; }

        protected DataPage(string name) {
            Name = name;
        }

        public void SaveChanges() {
            Parent.Database.AddChange(new ChangeSet(this, ChangeType.Update));
        }

        public void AddChild(string name, string schemaName, string data) {
            var page = new DataPage(name) {
                SchemaName = schemaName,
                PageData = data
            };

            Children.Add(page);
            Parent.Database.SaveChanges();
        }
    }
}
