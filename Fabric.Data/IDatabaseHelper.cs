using System;
using System.Collections;
using System.Collections.Generic;

namespace Fabric.Data {
    public interface IDatabaseHelper {

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        /// <returns>True if the database did not exist prior.</returns>
        bool Initialise();

        /// <summary>
        /// Writes the page.
        /// </summary>
        /// <param name="page">The page.</param>
        void WritePage(DataPage page);

        /// <summary>
        /// Reads the page.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        DataPage ReadPage(string path);

        /// <summary>
        /// Deletes the page.
        /// </summary>
        /// <param name="path">The path.</param>
        void DeletePage(string path);

        /// <summary>
        /// Writes the schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        void WriteSchema(DataPageSchema schema);

        /// <summary>
        /// Reads the schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns></returns>
        DataPageSchema ReadSchema(string schemaName);

        /// <summary>
        /// Deletes the schema.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        void DeleteSchema(string schemaName);

        /// <summary>
        /// Writes the page version.
        /// </summary>
        /// <param name="dataPageVersion">The data page version.</param>
        void WritePageVersion(DataPageVersion dataPageVersion);

        /// <summary>
        /// Reads the page version.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="versionGuid">The version unique identifier.</param>
        /// <returns></returns>
        DataPageVersion ReadPageVersion(string path, Guid versionGuid);

        /// <summary>
        /// Gets the page versions.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        IEnumerable<DataPageVersion> GetPageVersions(DataPage page);

        /// <summary>
        /// Gets the page version.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="versionGuid">The version unique identifier.</param>
        /// <returns></returns>
        DataPageVersion GetPageVersion(DataPage page, Guid versionGuid);

        /// <summary>
        /// Writes the schema version.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        void WriteSchemaVersion(DataPageSchema dataPage);

        /// <summary>
        /// Reads the schema version.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="versionGuid">The version unique identifier.</param>
        /// <returns></returns>
        DataPageSchema ReadSchemaVersion(string path, Guid versionGuid);
    }
}
