namespace Fabric.Data {
    public struct ChangeSet {
        internal DataPage ChangedPage { get; }

        internal DataPage ChangedParentPage => ChangedPage.ParentPage;

        internal ChangeType ChangeType { get; }

        public static ChangeSet Insert(DataPage changedPage) {
            return new ChangeSet(changedPage, ChangeType.Insert);
        }

        public static ChangeSet Update(DataPage changedPage) {
            return new ChangeSet(changedPage, ChangeType.Update);
        }

        public static ChangeSet Delete(DataPage changedPage) {
            return new ChangeSet(changedPage, ChangeType.Delete);
        }

        private ChangeSet(DataPage changedPage, ChangeType changeType) {
            ChangedPage = changedPage;
            ChangeType = changeType;
        }
    }
}
