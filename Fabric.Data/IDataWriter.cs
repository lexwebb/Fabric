using Newtonsoft.Json;

namespace Fabric.Data {
    public interface IDataWriter {
        JsonSerializerSettings SerializerSettings { get; }

        string DatabaseRoot { get; }

        /// <summary>
        ///     Writes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="data">The data.</param>
        void WriteFile(string path, string data = null);

        /// <summary>
        /// Appends the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="data">The data.</param>
        void AppendFile(string path, string data);

        /// <summary>
        ///     Writes the page.
        /// </summary>
        /// <param name="data">The data.</param>
        void WritePage(DataPage data);

        /// <summary>
        ///     Writes the page.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="data">The data.</param>
        void WritePage(string path,  DataPage data);

        /// <summary>
        ///     Deletes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        void DeleteFile(string path);

        /// <summary>
        ///     Deletes the folder.
        /// </summary>
        /// <param name="path">The path.</param>
        void DeleteFolder(string path);

        /// <summary>
        ///     Creates the folder.
        /// </summary>
        /// <param name="path">The path.</param>
        void CreateFolder(string path);
    }
}
