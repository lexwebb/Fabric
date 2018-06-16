using System.IO;
using Newtonsoft.Json;

namespace Fabric.Data {
    public class DataReader : IDataReader {
        public DataReader(JsonSerializerSettings serializerSettings, FabricDatabase database)
        {
            SerializerSettings = serializerSettings;
            DatabaseRoot = database.DatabaseRoot;
        }

        public JsonSerializerSettings SerializerSettings { get; }
        public string DatabaseRoot { get; }

        public string ReadFile(string path) {
            return File.ReadAllText(path);
        }

        public DataPage ReadPage(string path) {
            if (!path.StartsWith(DatabaseRoot))
            {
                path = Path.Combine(DatabaseRoot, path);
            }

            return JsonConvert.DeserializeObject<DataPage>(ReadFile(path), SerializerSettings);
        }

        public bool FileExists(string path) {
            return File.Exists(path);
        }

        public bool FolderExists(string path) {
            return Directory.Exists(path);
        }
    }
}
