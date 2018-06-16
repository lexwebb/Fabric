using System;

namespace Fabric.Data {
    public class ItemNotFoundException : Exception {
        private const string ErrorMessage = "Could not find item at specified path";

        public ItemNotFoundException(string itemName, string searchPath = null) : base(ErrorMessage) {
            ItemName = itemName;
            SearchPath = searchPath;
        }

        public string ItemName { get; set; }

        public string SearchPath { get; set; }
    }
}
