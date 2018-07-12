using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fabric.Data {
    public class VersionHelper : IVersionHelper {
        public VersionHelper(FabricDatabase fabricDatabase, IDatabaseHelper databaseHelper) {
            DatabaseHelper = databaseHelper;
            DatabaseRoot = fabricDatabase.DatabaseRoot;
        }
        
        public IDatabaseHelper DatabaseHelper { get; }

        public string DatabaseRoot { get; }

        /// <summary>
        /// Saves the version.
        /// </summary>
        /// <param name="page">The page.</param>
        public void SaveVersion(DataPage page) {
            var versions = GetVersions(page).ToArray();
            var versionNumber = versions.Any() ? versions.Max(p => p.VersionNumber) + 1 : 1;

            var versionObject = new DataPageVersion(
                Guid.NewGuid(),
                Utils.GetDataPagePath(page),
                DateTime.Now,
                versionNumber,
                page);

            DatabaseHelper.WritePageVersion(versionObject);
        }

        /// <summary>
        /// Gets the versions.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public IEnumerable<DataPageVersion> GetVersions(DataPage page) {
            return DatabaseHelper.GetPageVersions(page);
        }

        /// <summary>
        /// Gets the version information.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="versionGuid">The version unique identifier.</param>
        /// <returns></returns>
        /// <exception cref="ItemNotFoundException"></exception>
        public DataPageVersion GetVersionInformation(DataPage page, Guid versionGuid) {
            return DatabaseHelper.GetPageVersion(page, versionGuid);
        }
    }
}
