using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fabric.Data
{
    public class DataPageCollection : IEnumerable<DataPage> {
        public Dictionary<string, List<string>> GetNames() => _internalNameList;

        public string ModifiedTimestamp { get; internal set; }

        internal DataPage Parent { get; set; }

        internal FabricDatabase Database { get; set; }

        internal bool Dirty { get; set; } = false;

        internal DataPageCollection(FabricDatabase database, DataPage parent) {
            this.Database = database;
            this.Parent = parent;
        }

        /// <summary>
        /// Populates from serializer.
        /// </summary>
        /// <param name="names">The names.</param>
        internal void PopulateFromSerializer(Dictionary<string, List<string>> names) {
            _internalList.Clear();
            _internalNameList.Clear();
            _internalNameList = names ?? new Dictionary<string, List<string>>();
        }

        private List<DataPage> _internalList = new List<DataPage>();
        private Dictionary<string, List<string>> _internalNameList = new Dictionary<string, List<string>>();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<DataPage> GetEnumerator() {
            Load();
            return _internalList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            Load();
            return _internalList.GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        internal void Add(DataPage item) {
            item.Parent = this;
            DateTime.Now.GetTimestamp();
            item.Children = new DataPageCollection(Database, Parent);

            _internalList.Add(item);
            
            var exists = _internalNameList.TryGetValue(item.SchemaName, out var subList);

            if (!exists) {
                subList = new List<string>();
                _internalNameList.Add(item.SchemaName, subList);
            }

            subList?.Add(item.Name);

            this.Dirty = true;
            Database.AddChange(ChangeSet.Insert(item));
        }

        /// <summary>
        /// Deletes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if <paramref name="item">item</paramref> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if <paramref name="item">item</paramref> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        internal bool Delete(DataPage item) {
            var result = _internalList.Remove(item);

            _internalNameList.TryGetValue(item.SchemaName, out var subList);

            var result2 = subList?.Remove(item.Name);

            if (result && result2.HasValue && result2.Value) {
                this.Dirty = true;
                Database.AddChange(ChangeSet.Delete(item));
            }

            return result && result2.HasValue && result2.Value;
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        private void Load() {
            if(Dirty) return;

            _internalList.Clear();
            foreach (var childGroup in _internalNameList) {
                foreach (var child in childGroup.Value) {
                    var childPath = Path.Combine(Utils.GetDataPagePath(Parent), childGroup.Key, child, "dataPage.json");
                    _internalList.Add(Database.LoadPage(childPath));
                }
            }

            this._internalNameList.Clear();

            foreach (var dataPage in _internalList) {
                var name = dataPage.GetType().Name;

                var exists = _internalNameList.TryGetValue(name, out var subList);

                if (!exists) {
                    subList = new List<string>();
                    _internalNameList.Add(name, subList);
                }

                subList.Add(name);
            }
        }
    }
}
