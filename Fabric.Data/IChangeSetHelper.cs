namespace Fabric.Data {
    public interface IChangeSetHelper {

        /// <summary>
        ///     Adds the change.
        /// </summary>
        /// <param name="change">The change.</param>
        void AddChange(ChangeSet change);

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
        void Insert(string changesTimestamp, ChangeSet changeSet);

        /// <summary>
        ///     Saves the changes.
        /// </summary>
        void SaveChanges();
    }
}
