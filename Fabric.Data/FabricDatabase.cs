﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fabric.Logging;
using Newtonsoft.Json;

namespace Fabric.Data
{
    public class FabricDatabase
    {
        public string DatabaseRoot { get; }

        public string DatabaseFile => Path.Combine(DatabaseRoot, "FabricDatabase.json");

        public Action<FabricDatabase> SeedDatabase { get; set; }

        public RootPage Data { get; private set; }

        internal List<ChangeSet> Changes { get; } = new List<ChangeSet>();

        private readonly JsonSerializerSettings _serializerSettings;

        public FabricDatabase(string databaseRoot) {
            this.DatabaseRoot = databaseRoot;

            _serializerSettings = new JsonSerializerSettings() {
                Converters = new List<JsonConverter>() {
                    new DataPageSerializer(this)
                },
                Formatting = Formatting.Indented
            };

            Initialise();
        }

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException"></exception>
        private void Initialise() {
            Global.Instance.Logger.Info("Initialising database");

            if (!Directory.Exists(DatabaseRoot)) {
                var error = $"Could not find directory scedified to store DB: {DatabaseRoot}";
                Global.Instance.Logger.Error(error);
                throw new DirectoryNotFoundException(error);
            }

            if (!File.Exists(DatabaseFile)) {
                Global.Instance.Logger.Info("Database not found, performing first time setup");
                CreateDatabase();
                InternalSeedDatabase();
            }
            else {
                LoadDatabase();
            }

            Global.Instance.Logger.Info("Finished initialising database");
        }

        /// <summary>
        /// Creates the database.
        /// </summary>
        private void CreateDatabase() {
            Global.Instance.Logger.Info("Creating database file");
            File.Create(DatabaseFile).Close();

            var settings = new JsonSerializerSettings() {
                Converters = new List<JsonConverter>() {
                    new DataPageSerializer(this)
                },
                Formatting = Formatting.Indented
            };

            Data = new RootPage {
                ModifiedTimestamp = DateTime.Now.GetTimestamp()
            };

            Data.Children = new DataPageCollection(this, Data);

            var json = JsonConvert.SerializeObject(Data, settings);

            File.WriteAllText(DatabaseFile, json);
        }

        private void LoadDatabase() {
            Global.Instance.Logger.Info("Loading database file");

            var settings = new JsonSerializerSettings() {
                Converters = new List<JsonConverter>() {
                    new DataPageSerializer(this)
                },
                Formatting = Formatting.Indented
            };

            Data = JsonConvert.DeserializeObject<RootPage>(File.ReadAllText(DatabaseFile), settings);
        }

        /// <summary>
        /// Seeds the database.
        /// </summary>
        private void InternalSeedDatabase() {
            SeedDatabase?.Invoke(this);
        }

        internal List<DataPage> Load(DataPage parent) {
            var parts = FindParentsRecursive(Data, new List<string>());

            return new List<DataPage>();
        }

        private List<string> FindParentsRecursive(DataPage parent, List<string> partsList) {
            while (true) {
                if (parent?.Parent == null) return partsList;
                partsList.Add(parent.Name);
                parent = parent.Parent.Parent;
            }
        }

        public void SaveChanges() {
            var changesTimestamp = DateTime.Now.GetTimestamp();

            foreach (var changeSet in Changes.Where(c => c.ChangedPage != null)) {
                var parts = FindParentsRecursive(changeSet.ChangedPage, new List<string>());
                switch (changeSet.ChangeType) {
                    case ChangeType.Update:

                        break;
                    default:
                        throw new InvalidOperationException($"Invalid changeset type: {changeSet.ChangeType} for {nameof(DataPage)}");
                }
            }

            foreach (var changeSet in Changes.Where(c => c.ChangedCollection != null)) {
                var parts = FindParentsRecursive(changeSet.ChangedCollection.Parent, new List<string>());
                parts.Reverse();

                var path = parts.Prepend(DatabaseRoot).ToArray();

                switch (changeSet.ChangeType) {
                    case ChangeType.Insert:
                        foreach (var dataPageGroup in changeSet.ChangedCollection.GroupBy(d => d.GetType().Name)) {
                            var folderPath = Path.Combine(path.Append(dataPageGroup.Key).ToArray());

                            if(!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);

                            foreach (var dataPage in dataPageGroup) {
                                var pageFolderPath = Path.Combine(path.Append(dataPageGroup.Key).Append(dataPage.Name).ToArray());

                                if (!Directory.Exists(pageFolderPath))
                                    Directory.CreateDirectory(pageFolderPath);

                                var filePath = Path.Combine(pageFolderPath, "dataPage.json");

                                if (!File.Exists(filePath))
                                    File.Create(filePath).Close();

                                dataPage.ModifiedTimestamp = changesTimestamp;

                                File.WriteAllText(filePath, JsonConvert.SerializeObject(dataPage, _serializerSettings));
                            }
                        }
                        break;
                    case ChangeType.Delete:
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid changeset type: {changeSet.ChangeType} for {nameof(DataPageCollection)}");
                }

                var collectionRootFilePath = Path.Combine(path.Append("dataPage.json").ToArray());

                if(changeSet.ChangedCollection.Parent is RootPage)
                    collectionRootFilePath = Path.Combine(path.Append("FabricDatabase.json").ToArray());

                File.WriteAllText(collectionRootFilePath, JsonConvert.SerializeObject(changeSet.ChangedCollection.Parent, _serializerSettings));
            }
        }

        internal void AddChange(ChangeSet changeSet) {
            this.Changes.Add(changeSet);
        }
    }
}