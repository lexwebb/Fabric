using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fabric.Data {
    public class DataPageCollection : IEnumerable<DataPage> {
        private readonly List<DataPage> _internalList = new List<DataPage>();
        private Dictionary<string, List<string>> _internalNameList = new Dictionary<string, List<string>>();

        internal DataPageCollection(DataPage parent, IChangeSetHelper changeSetHelper, IDatabaseHelper databaseHelper) {
            ChangeSetHelper = changeSetHelper;
            DatabaseHelper = databaseHelper;
            Parent = parent;
        }

        public string ModifiedTimestamp { get; internal set; }

        internal DataPage Parent { get; set; }

        internal IChangeSetHelper ChangeSetHelper { get; set; }

        internal IDatabaseHelper DatabaseHelper { get; set; }

        internal bool Dirty { get; set; }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<DataPage> GetEnumerator() {
            Load();
            return _internalList.GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            Load();
            return _internalList.GetEnumerator();
        }

        /// <summary>
        ///     Gets the names.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetNames() {
            return _internalNameList.Count == 0 ? null : _internalNameList;
        }

        /// <summary>
        ///     Populates from serializer.
        /// </summary>
        /// <param name="names">The names.</param>
        internal void PopulateFromSerializer(Dictionary<string, List<string>> names) {
            _internalList.Clear();
            _internalNameList.Clear();
            _internalNameList = names ?? new Dictionary<string, List<string>>();
        }

        /// <summary>
        ///     Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        internal void Add(DataPage item) {
            item.Parent = this;
            DateTime.Now.GetTimestamp();
            item.Children = new DataPageCollection(Parent, ChangeSetHelper, DatabaseHelper);

            _internalList.Add(item);

            var exists = _internalNameList.TryGetValue(item.SchemaName, out var subList);

            if (!exists) {
                subList = new List<string>();
                _internalNameList.Add(item.SchemaName, subList);
            }

            subList?.Add(item.Name);

            Dirty = true;
            ChangeSetHelper.AddChange(ChangeSet.Insert(item));
        }

        /// <summary>
        ///     Deletes the first occurrence of a specific object from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        ///     true if <paramref name="item">item</paramref> was successfully removed from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if
        ///     <paramref name="item">item</paramref> is not found in the original
        ///     <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        internal bool Delete(DataPage item) {
            var itemRemoveResult = _internalList.Remove(item);

            _internalNameList.TryGetValue(item.SchemaName, out var nameListFiltered);

            var nameListRemoveResult = nameListFiltered?.Remove(item.Name);

            if (itemRemoveResult && nameListRemoveResult.HasValue && nameListRemoveResult.Value) {
                Dirty = true;
                ChangeSetHelper.AddChange(ChangeSet.Delete(item));
            }

            if (nameListFiltered?.Count == 0) {
                _internalNameList.Remove(item.SchemaName);
            }

            return itemRemoveResult && nameListRemoveResult.HasValue && nameListRemoveResult.Value;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        private void Load() {
            if (Dirty) {
                return;
            }

            var path = Utils.GetDataPagePath(Parent);

            _internalList.Clear();
            foreach (var childGroup in _internalNameList) {
                foreach (var child in childGroup.Value) {
                    var childPath = Path.Combine(
                        path ?? throw new InvalidOperationException("Could not find colleciton path"), childGroup.Key,
                        child);

                    _internalList.Add(DatabaseHelper.ReadPage(childPath));
                }
            }

            _internalNameList.Clear();

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

        /// <summary>
        /// Determines whether the data page collection contains any pages that have the given schema name.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>
        ///   <c>true</c> if the data page collection contains any pages that have the given schema name; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsAnyOfSchema(string schemaName) {
            return _internalNameList.Any(s => s.Key == schemaName);
        }
    }
}
