namespace Fabric.Data {
    public struct ChangeSet {
        internal DataPage ChangedPage { get; }

        internal DataPage ChangedParentPage => ChangedPage.ParentPage;

        internal ChangeType ChangeType { get; }

        /// <summary>
        ///     Creates an instance of ChangeSet that specifys an Insert action
        /// </summary>
        /// <param name="changedPage">The changed page.</param>
        /// <returns></returns>
        public static ChangeSet Insert(DataPage changedPage) {
            return new ChangeSet(changedPage, ChangeType.Insert);
        }

        /// <summary>
        ///     Creates an instance of ChangeSet that specifys an Update action
        /// </summary>
        /// <param name="changedPage">The changed page.</param>
        /// <returns></returns>
        public static ChangeSet Update(DataPage changedPage) {
            return new ChangeSet(changedPage, ChangeType.Update);
        }

        /// <summary>
        ///     Creates an instance of ChangeSet that specifys an Delete action
        /// </summary>
        /// <param name="changedPage">The changed page.</param>
        /// <returns></returns>
        public static ChangeSet Delete(DataPage changedPage) {
            return new ChangeSet(changedPage, ChangeType.Delete);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeSet" /> struct.
        /// </summary>
        /// <param name="changedPage">The changed page.</param>
        /// <param name="changeType">Type of the change.</param>
        private ChangeSet(DataPage changedPage, ChangeType changeType) {
            ChangedPage = changedPage;
            ChangeType = changeType;
        }
    }
}