using System;
using System.Collections.Generic;

namespace Fabric.Data {
    public interface IVersionHelper {
        /// <summary>
        /// Saves the version.
        /// </summary>
        /// <param name="page">The page.</param>
        void SaveVersion(DataPage page);

        /// <summary>
        /// Gets the versions.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        IEnumerable<DataPageVersion> GetVersions(DataPage page);

        /// <summary>
        /// Gets the version information.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="versionGuid">The version unique identifier.</param>
        /// <returns></returns>
        DataPageVersion GetVersionInformation(DataPage page, Guid versionGuid);
    }
}
