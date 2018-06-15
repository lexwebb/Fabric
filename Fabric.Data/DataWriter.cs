using System;
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

        public void WriteFile(string path, string data = null) {
            var stream = File.Exists(path) ? File.Open(path, FileMode.Truncate) : File.Create(path);

            if (data != null)
            {
                var info = new UTF8Encoding(true).GetBytes(data);
                stream.Write(info, 0, info.Length);
            }

            stream.Close();
        }

        public void WritePage(DataPage data) {
            WriteFile(Path.Combine(DatabaseRoot, Utils.GetDataPagePath(data)), JsonConvert.SerializeObject(data, SerializerSettings));
        }

        public void DeleteFile(string path) {
            if (!path.StartsWith(DatabaseRoot))
            {
                path = Path.Combine(DatabaseRoot, path);
            }

            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        public void DeleteFolder(string path) {
            if (!path.StartsWith(DatabaseRoot))
            {
                path = Path.Combine(DatabaseRoot, path);
            }

            if (Directory.Exists(path)) {
                Directory.Delete(path, true);
            }
        }

        public void CreateFolder(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }
    }
}
