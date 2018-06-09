using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Fabric.Data {
    public class ChangeSetHelper : IChangeSetHelper {
        public string GetPageFilePathFromChangeSet(FabricDatabase database, ChangeSet changeSet) {
            var parts = Utils.FindParentsRecursive(changeSet.ChangedPage.Parent.Parent, new List<string>());
            parts.Reverse();

            if (parts[0] == "root") {
                parts = parts.Skip(1).ToList();
            }

            parts.Add(database.FullDataBaseRoot);
            parts.Add(changeSet.ChangedPage.SchemaName);
            parts.Add(changeSet.ChangedPage.Name);

            var dirPath = Path.Combine(parts.ToArray());

            return Path.Combine(dirPath, "dataPage.json");
        }

        public void Update(FabricDatabase database, string changesTimestamp, ChangeSet changeSet) {
            var filePath = GetPageFilePathFromChangeSet(database, changeSet);
            changeSet.ChangedPage.ModifiedTimestamp = changesTimestamp;

            File.WriteAllText(filePath,
                JsonConvert.SerializeObject(changeSet.ChangedPage, database.SerializerSettings));
        }

        public void Delete(FabricDatabase database, string changesTimestamp, ChangeSet changeSet) {
            var filePath = GetPageFilePathFromChangeSet(database, changeSet);
            var folderPath = Path.GetDirectoryName(filePath);

            if (folderPath == null) {
                throw new InvalidOperationException("Data page does not exist");
            }

            Directory.Delete(folderPath, true);
        }

        public void Insert(FabricDatabase database, string changesTimestamp, ChangeSet changeSet,
            string[] collectionPath) {
            var dataPage = changeSet.ChangedPage;

            var folderPath = Path.Combine(collectionPath.Append(dataPage.SchemaName).ToArray());

            if (!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }

            var pageFolderPath =
                Path.Combine(collectionPath.Append(dataPage.SchemaName).Append(dataPage.Name).ToArray());

            if (!Directory.Exists(pageFolderPath)) {
                Directory.CreateDirectory(pageFolderPath);
            }

            var filePath = Path.Combine(pageFolderPath, "dataPage.json");

            if (!File.Exists(filePath)) {
                File.Create(filePath).Close();
            }

            dataPage.ModifiedTimestamp = changesTimestamp;

            File.WriteAllText(filePath, JsonConvert.SerializeObject(dataPage, database.SerializerSettings));
        }

        public void SaveCollectionChanges(ChangeSet changeSet, string[] collectionPath, FabricDatabase database) {
            var collectionRootFilePath = Path.Combine(collectionPath.Append("dataPage.json").ToArray());

            if (changeSet.ChangedPage.Parent.Parent is RootPage) {
                collectionRootFilePath = Path.Combine(collectionPath.Append("FabricDatabase.json").ToArray());
            }

            File.WriteAllText(collectionRootFilePath,
                JsonConvert.SerializeObject(changeSet.ChangedPage, database.SerializerSettings));
        }
    }
}
