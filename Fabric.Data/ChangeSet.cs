using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Data
{
    internal struct ChangeSet
    {
        internal DataPage ChangedPage { get; }

        internal DataPageCollection ChangedCollection { get; }

        internal ChangeType ChangeType { get; }

        public ChangeSet(DataPage changedPage, ChangeType changeType) {
            this.ChangedPage = changedPage;
            this.ChangedCollection = null;
            this.ChangeType = changeType;
        }

        public ChangeSet(DataPageCollection changedCollection, ChangeType changeType) {
            this.ChangedPage = null;
            this.ChangedCollection = changedCollection;
            this.ChangeType = changeType;
        }
    }
}
