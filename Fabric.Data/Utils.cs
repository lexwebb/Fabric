using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fabric.Data {
    internal static class Utils {
        internal static bool IsPathCollection(string path) {
            path = path?.TrimEnd('/');
            if (string.IsNullOrEmpty(path)) {
                return false;
            }

            return path.Split('/').Length % 2 != 0;
        }

        internal static List<string> FindParentsRecursive(DataPage parent, List<string> partsList) {
            while (true) {
                if (parent?.Parent == null) {
                    return partsList;
                }

                partsList.Add(parent.Name);
                parent = parent.Parent.Parent;
            }
        }

        internal static async Task<DataPage> FindChildPage(DataPage root, string path) {
            path = path?.TrimEnd('/');
            try {
                if (IsPathCollection(path)) {
                    throw new ArgumentException("Provided path points to a collection not a page.");
                }

                return await Task.Run(() => FindChildPageReccursive(root, path));
            }
            catch (ItemNotFoundException ex) {
                throw new ItemNotFoundException(ex.ItemName, path);
            }
        }

        internal static DataPage FindChildPageReccursive(DataPage root, string path) {
            path = path?.TrimEnd('/');
            if (path == null || path.Equals(string.Empty)) {
                return root;
            }

            var pathParts = path.Split('/');
            var schemaName = pathParts[0];
            var itemName = pathParts[1];

            if (root.Children.Any(c => c.Name == itemName)) {
                var currentPage = root.Children.First(c => c.SchemaName == schemaName && c.Name == itemName);
                return pathParts.Length > 2
                    ? FindChildPageReccursive(currentPage, string.Join("/", pathParts.Skip(2)))
                    : currentPage;
            }

            throw new ItemNotFoundException(itemName);
        }

        internal static async Task<IEnumerable<DataPage>> FindChildCollection(DataPage root, string path) {
            path = path?.TrimEnd('/');
            try {
                if (!IsPathCollection(path)) {
                    throw new ArgumentException("Provided path points to a page not a collection.");
                }

                return await Task.Run(() => FindChildCollectionRecursive(root, path));
            }
            catch (ItemNotFoundException ex) {
                throw new ItemNotFoundException(ex.ItemName, path);
            }
        }

        internal static IEnumerable<DataPage> FindChildCollectionRecursive(DataPage root, string path) {
            path = path?.TrimEnd('/');
            if (path == null || path.Equals(string.Empty)) {
                throw new ItemNotFoundException(path);
            }

            var pathParts = path.Split('/');
            var currentPathRoot = pathParts[0];

            if (root.Children.Any(c => c.SchemaName == currentPathRoot)) {
                if (pathParts.Length == 1) {
                    return root.Children.Where(c => c.SchemaName == currentPathRoot);
                }

                var nextCollectionName = pathParts[2];
                var itemName = pathParts[1];

                if (root.Children.Any(c => c.SchemaName == currentPathRoot && c.Name == itemName)) {
                    var item = root.Children.First(c => c.SchemaName == currentPathRoot && c.Name == itemName);
                    return FindChildCollectionRecursive(item, nextCollectionName);
                }
            }

            throw new ItemNotFoundException(currentPathRoot);
        }

        internal static string GetDataPagePath(DataPage dataPage) {
            if (dataPage.Name == "root" && dataPage.Parent.Parent == null) {
                return "FabricDatabase.json";
            }

            var parts = FindParentsRecursive(dataPage.Parent.Parent, new List<string>());
            parts.Reverse();

            if (parts[0] == "root") {
                parts = parts.Skip(1).ToList();
            }

            parts.Add(dataPage.SchemaName);
            parts.Add(dataPage.Name);

            var dirPath = Path.Combine(parts.ToArray());

            return Path.Combine(dirPath, "dataPage.json");
        }
    }
}