using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fabric.Data
{
    public class ChildPageCollection<T> : IList<T> where T : DataPage
    {
        public string[] GetNames() => _internalNameList.ToArray();

        internal void PopulateFromSerializer(string[] names) {
            this.Clear();
            _internalNameList.AddRange(names);
            _hasFullList = false;
        }

        private void CheckFullList() {
            if(!_hasFullList)
                throw new InvalidOperationException("Collection has not been fully loaded, and full list is not avialable.");
        }

        private readonly List<T> _internalList = new List<T>();
        private readonly List<string> _internalNameList = new List<string>();
        private bool _hasFullList = true;

        public IEnumerator<T> GetEnumerator() {
            return _internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _internalList.GetEnumerator();
        }

        public void Add(T item) {
            CheckFullList();
            _internalList.Add(item);
            _internalNameList.Add(item.Name);
        }

        public void Clear() {
            _internalList.Clear();
            _internalNameList.Clear();
        }

        public bool Contains(T item) {
            CheckFullList();
            return _internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            CheckFullList();
            _internalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            CheckFullList();
            var result = _internalList.Remove(item);
            var result2 = _internalNameList.Remove(item.Name);

            return result && result2;
        }

        public int Count => _internalList.Count;
        public bool IsReadOnly => false;

        public int IndexOf(T item) {
            CheckFullList();
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, T item) {
            CheckFullList();
            _internalList.Insert(index, item);
            _internalNameList.Insert(index, item.Name);
        }

        public void RemoveAt(int index) {
            CheckFullList();
            _internalList.RemoveAt(index);
            _internalNameList.RemoveAt(index);
        }

        public T this[int index] {
            get => _internalList[index];
            set => _internalList[index] = value;
        }
    }
}
