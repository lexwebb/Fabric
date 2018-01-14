﻿using System.Collections.Generic;

namespace Fabric.Data {
    public interface ISchemaManager : IEnumerable<DataPageSchema> {
        List<DataPageSchema> Schemas { get; }

        void Add(string schemaName, string schemaRawJson);

        void Update(string schemaName, string schemaRawJson);

        void Delete(string schemaName);

        void LoadSchemas();
    }
}