using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fabric.Data {
    public class VersionHelper : IVersionHelper {
        public VersionHelper(FabricDatabase fabricDatabase, IDataWriter dataWriter, IDataReader dataReader) {
            DataWriter = dataWriter;
            DataReader = dataReader;
            DatabaseRoot = fabricDatabase.DatabaseRoot;

            Initialise();
        }

        public IDataWriter DataWriter { get; }
        public IDataReader DataReader { get; }

        public string DatabaseRoot { get; }

        public string VersionsRootPath => Path.Combine(DatabaseRoot, FabricDatabase.DataPageVersionDirName);
        public string VersionsFilePath => Path.Combine(VersionsRootPath, FabricDatabase.DataPageVersionFileName);

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        public void Initialise() {
            DataWriter.WriteFile(VersionsFilePath);
        }

        /// <summary>
        /// Saves the version.
        /// </summary>
        /// <param name="page">The page.</param>
        public void SaveVersion(DataPage page) {
            var versions = GetVersions(page).ToArray();
            var versionNumber = versions.Any() ? versions.Max(p => p.VersionNumber) + 1 : 1;

            var versionObject = new DataPageVersion(
                Guid.NewGuid(),
                Utils.GetDataPagePath(page),
                DateTime.Now,
                versionNumber);

            var versionJson = JObject.FromObject(versionObject);

            var value = versionJson.ToString(Formatting.None);
            DataWriter.AppendFile(VersionsFilePath, $"{versionObject.DataPagePath}:{value}");
            DataWriter.WritePage(Path.Combine(VersionsRootPath, $"{versionObject.VersionGuid}.json"), page);
        }

        /// <summary>
        /// Gets the versions.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public IEnumerable<DataPageVersion> GetVersions(DataPage page) {
            using (var fileStream = DataReader.OpenFileStream(VersionsFilePath)) {
                using (var streamReader = new StreamReader(fileStream)) {
                    while (!streamReader.EndOfStream) {
                        var line = streamReader.ReadLine();
                        if (line != null && line.StartsWith(Utils.GetDataPagePath(page))) {
                            var data = line.Substring(line.IndexOf(":", StringComparison.Ordinal) + 1);
                            yield return JsonConvert.DeserializeObject<DataPageVersion>(data);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the version information.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="versionGuid">The version unique identifier.</param>
        /// <returns></returns>
        /// <exception cref="ItemNotFoundException"></exception>
        public DataPageVersion GetVersionInformation(DataPage page, Guid versionGuid) {
            using (var fileStream = DataReader.OpenFileStream(VersionsFilePath)) {
                using (var streamReader = new StreamReader(fileStream)) {
                    while (!streamReader.EndOfStream) {
                        var line = streamReader.ReadLine();
                        if (line != null && line.StartsWith(Utils.GetDataPagePath(page))) {
                            var data = line.Substring(line.IndexOf(":", StringComparison.Ordinal));
                            var version = JsonConvert.DeserializeObject<DataPageVersion>(data);
                            if (version.VersionGuid == versionGuid) {
                                return version;
                            }
                        }
                    }
                }
            }

            throw new ItemNotFoundException(versionGuid.ToString(), Utils.GetDataPagePath(page));
        }
    }
}
