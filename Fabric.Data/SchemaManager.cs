using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fabric.Data {
    public class SchemaManager : ISchemaManager {
        private const string SchemaSubFolderName = "schemas";

        private readonly FabricDatabase _database;

        public SchemaManager(FabricDatabase database, IDataWriter dataWriter, IDataReader dataReader) {
            DataWriter = dataWriter;
            DataReader = dataReader;
            _database = database;
            Schemas = new List<DataPageSchema>();

            if (!Directory.Exists(SchemaSubFolderPath)) {
                Directory.CreateDirectory(SchemaSubFolderPath);
            }
        }

        public IDataWriter DataWriter { get; }
        public IDataReader DataReader { get; }

        private string SchemaSubFolderPath => Path.Combine(_database.FullDataBaseRoot, SchemaSubFolderName);

        public IEnumerator<DataPageSchema> GetEnumerator() {
            return Schemas.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Schemas.GetEnumerator();
        }

        public List<DataPageSchema> Schemas { get; }

        public void Add(string schemaName, string schemaRawJson) {
            if (Schemas.Any(s => s.SchemaName == schemaName)) {
                throw new InvalidOperationException(
                    $"Unable to add schema. A schema with the name '{schemaName}' already exists");
            }

            Schemas.Add(new DataPageSchema(schemaName, schemaRawJson));
            DataWriter.WriteFile(Path.Combine(SchemaSubFolderPath, $"{schemaName}.json"),
                JsonUtils.Prettify(schemaRawJson));
        }

        public void Update(string schemaName, string schemaRawJson) {
            if (Schemas.All(s => s.SchemaName != schemaName)) {
                throw new InvalidOperationException(
                    $"Unable to update schema. No schema with the name '{schemaName}' exists");
            }

            Schemas[Schemas.FindIndex(s => s.SchemaName == schemaName)] = new DataPageSchema(schemaName, schemaRawJson);
            DataWriter.WriteFile(Path.Combine(SchemaSubFolderPath, $"{schemaName}.json"),
                JsonUtils.Prettify(schemaRawJson));
        }

        public void Delete(string schemaName) {
            if (Schemas.All(s => s.SchemaName != schemaName)) {
                throw new InvalidOperationException(
                    $"Unable to delete schema. No schema with the name '{schemaName}' exists");
            }

            Schemas.RemoveAt(Schemas.FindIndex(s => s.SchemaName == schemaName));
            DataWriter.DeleteFile(Path.Combine(SchemaSubFolderPath, $"{schemaName}.json"));
        }

        public void LoadSchemas() {
            var schemaFiles = Directory.GetFiles(SchemaSubFolderPath);

            foreach (var schemaFile in schemaFiles) {
                var json = DataReader.ReadFile(schemaFile);
;                Schemas.Add(new DataPageSchema(Path.GetFileNameWithoutExtension(schemaFile), json));
            }
        }
    }
}
