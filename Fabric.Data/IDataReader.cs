using Newtonsoft.Json;

namespace Fabric.Data {
    public interface IDataReader {
        JsonSerializerSettings SerializerSettings { get; }

        string ReadFile(string path);

        DataPage ReadPage(string path);

        bool FileExists(string path);

        bool FolderExists(string path);
    }
}
