using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Data
{
    public class ItemNotFoundException : Exception {
        private const string ErrorMessage = "Could not find item at specified path";

        public string ItemName { get; set; }

        public string SearchPath { get; set; }

        public ItemNotFoundException(string itemName, string searchPath = null) : base(ErrorMessage) {
            ItemName = itemName;
            SearchPath = searchPath;
        }
    }
}
