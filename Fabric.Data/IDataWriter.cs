using Newtonsoft.Json;

namespace Fabric.Data {
    public interface IDataWriter {
        JsonSerializerSettings SerializerSettings { get; }

        string DatabaseRoot { get; }

        void WriteFile(string path, string data = null);

        void WritePage(DataPage data);

        void DeleteFile(string path);

        void DeleteFolder(string path);

        void CreateFolder(string path);
    }
}