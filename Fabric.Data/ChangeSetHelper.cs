using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fabric.Data {
    public class ChangeSetHelper : IChangeSetHelper {
        private readonly object _changeSetLock = new object();

        public ChangeSetHelper(IDataWriter dataWriter, IDataReader dataReader) {
            DataWriter = dataWriter;
            DataReader = dataReader;
            Changes = new Queue<ChangeSet>();
        }

        private Queue<ChangeSet> Changes { get; }

        public IDataWriter DataWriter { get; }

        public IDataReader DataReader { get; }

        public void AddChange(ChangeSet change) {
            lock (_changeSetLock) {
                Changes.Enqueue(change);
            }
        }

        public void Update(string changesTimestamp, ChangeSet changeSet) {
            changeSet.ChangedPage.ModifiedTimestamp = changesTimestamp;

            DataWriter.WritePage(changeSet.ChangedPage);
        }

        public void Delete(string changesTimestamp, ChangeSet changeSet) {
            var filePath = Utils.GetDataPagePath(changeSet.ChangedPage);
            var folderPath = Path.GetDirectoryName(filePath);

            if (folderPath == null) {
                throw new InvalidOperationException("Data page does not exist");
            }

            DataWriter.DeleteFolder(folderPath);
            DataWriter.WritePage(changeSet.ChangedParentPage);
        }

        public void Insert(string changesTimestamp, ChangeSet changeSet) {
            var dataPage = changeSet.ChangedPage;

            dataPage.ModifiedTimestamp = changesTimestamp;

            DataWriter.WritePage(dataPage);
            DataWriter.WritePage(changeSet.ChangedParentPage);
        }

        /// <summary>
        ///     Saves the changes.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void SaveChanges() {
            var changesTimestamp = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());

            lock (_changeSetLock) {
                while (Changes.Any()) {
                    var changeSet = Changes.Dequeue();

                    switch (changeSet.ChangeType) {
                        case ChangeType.Update:
                            Update(changesTimestamp, changeSet);
                            break;
                        case ChangeType.Insert:
                            Insert(changesTimestamp, changeSet);
                            break;
                        case ChangeType.Delete:
                            Delete(changesTimestamp, changeSet);
                            break;
                        default:
                            throw new InvalidOperationException(
                                $"Invalid changeset type: {changeSet.ChangeType} for {nameof(DataPage)}");
                    }
                }
            }
        }
    }
}
