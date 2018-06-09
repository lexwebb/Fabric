using System.Collections.Generic;

namespace Fabric.Data {
    public interface IChangeSetHelper {
        /// <summary>
        /// Gets the page path from change set.
        /// </summary>
        /// <param name="changeSet">The change set.</param>
        /// <returns></returns>
        string GetPageFilePathFromChangeSet(FabricDatabase database, ChangeSet changeSet);

        /// <summary>
        /// Updates the page in the database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="changesTimestamp">The changes timestamp.</param>
        /// <param name="changeSet">The change set.</param>
        void Update(FabricDatabase database, string changesTimestamp, ChangeSet changeSet);

        /// <summary>
        /// Deletes the page from the database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="changesTimestamp">The changes timestamp.</param>
        /// <param name="changeSet">The change set.</param>
        void Delete(FabricDatabase database, string changesTimestamp, ChangeSet changeSet);

        /// <summary>
        /// Inserts the page into the database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="changesTimestamp">The changes timestamp.</param>
        /// <param name="changeSet">The change set.</param>
        /// <param name="collectionPath">The collection path.</param>
        void Insert(FabricDatabase database, string changesTimestamp, ChangeSet changeSet, string[] collectionPath);

        /// <summary>
        /// Saves the collection changes.
        /// </summary>
        /// <param name="collectionPath">The collection path.</param>
        void SaveCollectionChanges(ChangeSet changeSet, string[] collectionPath, FabricDatabase database);
    }
}
