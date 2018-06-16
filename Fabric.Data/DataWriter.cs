using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Fabric.Data {
    public class DataWriter : IDataWriter {
        public DataWriter(JsonSerializerSettings serializerSettings, FabricDatabase database) {
            SerializerSettings = serializerSettings;
            DatabaseRoot = database.DatabaseRoot;
        }

        public JsonSerializerSettings SerializerSettings { get; }
        public string DatabaseRoot { get; }

        /// <summary>
        ///     Writes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="data">The data.</param>
        public void WriteFile(string path, string data = null) {
            CreateFolder(Path.GetDirectoryName(path));
            var stream = File.Exists(path) ? File.Open(path, FileMode.Truncate) : File.Create(path);

            if (data != null) {
                var info = new UTF8Encoding(true).GetBytes(data);
                stream.Write(info, 0, info.Length);
            }

            stream.Close();
        }

        /// <summary>
        ///     Writes the page.
        /// </summary>
        /// <param name="data">The data.</param>
        public void WritePage(DataPage data) {
            var path = Utils.GetDataPagePath(data);
            WriteFile(Path.Combine(DatabaseRoot, path), JsonConvert.SerializeObject(data, SerializerSettings));
        }

        /// <summary>
        ///     Deletes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        public void DeleteFile(string path) {
            if (!path.StartsWith(DatabaseRoot)) {
                path = Path.Combine(DatabaseRoot, path);
            }

            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        /// <summary>
        ///     Deletes the folder.
        /// </summary>
        /// <param name="path">The path.</param>
        public void DeleteFolder(string path) {
            if (!path.StartsWith(DatabaseRoot)) {
                path = Path.Combine(DatabaseRoot, path);
            }

            if (Directory.Exists(path)) {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        ///     Creates the folder.
        /// </summary>
        /// <param name="path">The path.</param>
        public void CreateFolder(string path) {
            if (!Directory.Exists(path) && path != string.Empty) {
                Directory.CreateDirectory(path);
            }
        }
    }
}