using Newtonsoft.Json;

namespace Fabric.Data {
    public interface IDataReader {
        JsonSerializerSettings SerializerSettings { get; }

        string DatabaseRoot { get; }

        /// <summary>
        ///     Reads the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string ReadFile(string path);

        /// <summary>
        ///     Reads the page.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        DataPage ReadPage(string path);

        /// <summary>
        ///     Checks if the file exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        bool FileExists(string path);

        /// <summary>
        ///     Checks if the folder exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        bool FolderExists(string path);
    }
}