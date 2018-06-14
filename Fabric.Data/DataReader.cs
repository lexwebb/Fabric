using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Fabric.Data {
    public class DataReader : IDataReader {
        public DataReader(JsonSerializerSettings serializerSettings) {
            SerializerSettings = serializerSettings;
        }

        public JsonSerializerSettings SerializerSettings { get; }

        public string ReadFile(string path) {
            return File.ReadAllText(path);
        }

        public DataPage ReadPage(string path) {
            return JsonConvert.DeserializeObject<DataPage>(ReadFile(path));
        }

        public bool FileExists(string path) {
            return File.Exists(path);
        }

        public bool FolderExists(string path) {
            return Directory.Exists(path);
        }
    }
}
