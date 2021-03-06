﻿using System.Collections.Generic;
using System.Linq;

namespace Fabric.Data {
    public class DataPage {
        internal DataPage(string name) {
            Name = name;
        }

        internal DataPage() { }

        public string Name { get; internal set; }

        public string ModifiedTimestamp { get; internal set; }

        public string SchemaName { get; internal set; }

        public string PageData { get; set; }

        public DataPageCollection Children { get; set; }

        internal DataPageCollection Parent { get; set; }

        internal DataPage ParentPage => Parent.Parent;

        /// <summary>
        ///     Gets the children.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns></returns>
        public IEnumerable<DataPage> GetChildren(string schemaName = null) {
            return schemaName != null ? Children.Where(c => c.SchemaName == schemaName) : Children;
        }

        /// <summary>
        ///     Gets the child.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns></returns>
        /// <exception cref="ItemNotFoundException"></exception>
        public DataPage GetChild(string name, string schemaName = "none") {
            return GetChildren(schemaName).FirstOrDefault(c => c.Name == name) ?? throw new ItemNotFoundException(name);
        }

        /// <summary>
        ///     Saves the changes.
        /// </summary>
        public void SaveChanges() {
            Parent.ChangeSetHelper.AddChange(ChangeSet.Update(this));
            Parent.ChangeSetHelper.SaveChanges();
        }

        /// <summary>
        ///     Adds the child.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="data">The data.</param>
        public void AddChild(string name, string schemaName, string data) {
            var page = new DataPage(name) {
                SchemaName = schemaName,
                PageData = data
            };

            Children.Add(page);
            Parent.ChangeSetHelper.SaveChanges();
        }

        /// <summary>
        ///     Deletes the child.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="schemaName">Name of the schema.</param>
        public void DeleteChild(string name, string schemaName) {
            var page = Children.FirstOrDefault(c => c.Name == name && c.SchemaName == schemaName);
            Children.Delete(page);
            Parent.ChangeSetHelper.SaveChanges();
        }

        /// <summary>
        /// Deletes this page.
        /// </summary>
        public void Delete() {
            ParentPage.DeleteChild(Name, SchemaName);
        }
    }
}
