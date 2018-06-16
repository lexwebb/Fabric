using System.IO;
using Newtonsoft.Json;

namespace Fabric.Data {
    public class DataReader : IDataReader {
        public DataReader(JsonSerializerSettings serializerSettings, FabricDatabase database) {
            SerializerSettings = serializerSettings;
            DatabaseRoot = database.DatabaseRoot;
        }

        public JsonSerializerSettings SerializerSettings { get; }
        public string DatabaseRoot { get; }

        /// <summary>
        ///     Reads the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string ReadFile(string path) {
            return File.ReadAllText(path);
        }

        /// <summary>
        ///     Reads the page.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public DataPage ReadPage(string path) {
            if (!path.StartsWith(DatabaseRoot)) {
                path = Path.Combine(DatabaseRoot, path);
            }

            return JsonConvert.DeserializeObject<DataPage>(ReadFile(path), SerializerSettings);
        }

        /// <summary>
        ///     Checks if the file exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public bool FileExists(string path) {
            return File.Exists(path);
        }

        /// <summary>
        ///     Checks if the folder exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public bool FolderExists(string path) {
            return Directory.Exists(path);
        }
    }
}