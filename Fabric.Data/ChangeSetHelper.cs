using System;
using System.IO;
using System.Linq;

namespace Fabric.Data {
    public class ChangeSetHelper : IChangeSetHelper {
        public ChangeSetHelper(IDataWriter dataWriter, IDataReader dataReader) {
            DataWriter = dataWriter;
            DataReader = dataReader;
        }

        public IDataWriter DataWriter { get; }

        public IDataReader DataReader { get; }

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
            DataWriter.WritePage(changeSet.ChangedPage.Parent.Parent);
        }

        public void Insert(string changesTimestamp, ChangeSet changeSet,
            string[] collectionPath) {
            var dataPage = changeSet.ChangedPage;

            var folderPath = Path.Combine(collectionPath.Append(dataPage.SchemaName).ToArray());

            if (!DataReader.FolderExists(folderPath)) {
                DataWriter.CreateFolder(folderPath);
            }

            var pageFolderPath =
                Path.Combine(collectionPath.Append(dataPage.SchemaName).Append(dataPage.Name).ToArray());

            if (!DataReader.FolderExists(pageFolderPath)) {
                DataWriter.CreateFolder(pageFolderPath);
            }

            var filePath = Path.Combine(pageFolderPath, "dataPage.json");

            if (!DataReader.FileExists(filePath)) {
                DataWriter.WriteFile(filePath);
            }

            dataPage.ModifiedTimestamp = changesTimestamp;

            DataWriter.WritePage(dataPage);
            DataWriter.WritePage(changeSet.ChangedPage.Parent.Parent);
        }
    }
}
