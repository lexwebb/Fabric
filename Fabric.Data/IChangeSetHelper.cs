namespace Fabric.Data {
    public interface IChangeSetHelper {
        /// <summary>
        ///     Gets the data writer.
        /// </summary>
        /// <value>
        ///     The data writer.
        /// </value>
        IDataWriter DataWriter { get; }

        /// <summary>
        ///     Gets the data reader.
        /// </summary>
        /// <value>
        ///     The data reader.
        /// </value>
        IDataReader DataReader { get; }

        /// <summary>
        ///     Updates the page in the database.
        /// </summary>
        /// <param name="changesTimestamp">The changes timestamp.</param>
        /// <param name="changeSet">The change set.</param>
        void Update(string changesTimestamp, ChangeSet changeSet);

        /// <summary>
        ///     Deletes the page from the database.
        /// </summary>
        /// <param name="changesTimestamp">The changes timestamp.</param>
        /// <param name="changeSet">The change set.</param>
        void Delete(string changesTimestamp, ChangeSet changeSet);

        /// <summary>
        ///     Inserts the page into the database.
        /// </summary>
        /// <param name="changesTimestamp">The changes timestamp.</param>
        /// <param name="changeSet">The change set.</param>
        /// <param name="collectionPath">The collection path.</param>
        void Insert(string changesTimestamp, ChangeSet changeSet, string[] collectionPath);

        /// <summary>
        ///     Saves the collection changes.
        /// </summary>
        /// <param name="collectionPath">The collection path.</param>
        void SaveCollectionChanges(ChangeSet changeSet, string[] collectionPath);
    }
}
