using System.Collections.Generic;

namespace Fabric.Data {
    public interface ISchemaManager : IEnumerable<DataPageSchema> {
        List<DataPageSchema> Schemas { get; }
        
        IDatabaseHelper DatabaseHelper { get; }

        /// <summary>
        ///     Adds the specified schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="schemaRawJson">The schema raw json.</param>
        void Add(string schemaName, string schemaRawJson);

        /// <summary>
        ///     Updates the specified schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="schemaRawJson">The schema raw json.</param>
        void Update(string schemaName, string schemaRawJson);

        /// <summary>
        ///     Deletes the specified schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        void Delete(string schemaName);

        /// <summary>
        ///     Loads the schemas.
        /// </summary>
        void LoadSchemas();
    }
}
