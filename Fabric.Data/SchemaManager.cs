﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fabric.Data {
    public class SchemaManager : ISchemaManager {
        private const string SchemaSubFolderName = "schemas";

        public SchemaManager(FabricDatabase database, IDatabaseHelper databaseHelper) {
            DatabaseHelper = databaseHelper;
            SchemaSubFolderPath = Path.Combine(database.FullDataBaseRoot, SchemaSubFolderName);
            Schemas = new List<DataPageSchema>();

            if (!Directory.Exists(SchemaSubFolderPath)) {
                Directory.CreateDirectory(SchemaSubFolderPath);
            }
        }

        private string SchemaSubFolderPath { get; }
        
        public IDatabaseHelper DatabaseHelper { get; }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<DataPageSchema> GetEnumerator() {
            return Schemas.GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return Schemas.GetEnumerator();
        }

        public List<DataPageSchema> Schemas { get; }

        /// <summary>
        ///     Adds the specified schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="schemaRawJson">The schema raw json.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Add(string schemaName, string schemaRawJson) {
            if (Schemas.Any(s => s.SchemaName == schemaName)) {
                throw new InvalidOperationException(
                    $"Unable to add schema. A schema with the name '{schemaName}' already exists");
            }

            var schema = new DataPageSchema(schemaName, schemaRawJson);
            Schemas.Add(schema);
            DatabaseHelper.WriteSchema(schema);
        }

        /// <summary>
        ///     Updates the specified schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="schemaRawJson">The schema raw json.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Update(string schemaName, string schemaRawJson) {
            if (Schemas.All(s => s.SchemaName != schemaName)) {
                throw new InvalidOperationException(
                    $"Unable to update schema. No schema with the name '{schemaName}' exists");
            }
            
            var schema = new DataPageSchema(schemaName, schemaRawJson);
            Schemas[Schemas.FindIndex(s => s.SchemaName == schemaName)] = schema;
            DatabaseHelper.WriteSchema(schema);
        }

        /// <summary>
        ///     Deletes the specified schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Delete(string schemaName) {
            if (Schemas.All(s => s.SchemaName != schemaName)) {
                throw new InvalidOperationException(
                    $"Unable to delete schema. No schema with the name '{schemaName}' exists");
            }

            Schemas.RemoveAt(Schemas.FindIndex(s => s.SchemaName == schemaName));
            DatabaseHelper.DeleteSchema(schemaName);
        }

        /// <summary>
        ///     Loads the schemas.
        /// </summary>
        public void LoadSchemas() {
            var schemaFiles = Directory.GetFiles(SchemaSubFolderPath);

            foreach (var schemaFile in schemaFiles) {
                var schema = DatabaseHelper.ReadSchema(Path.GetFileNameWithoutExtension(schemaFile));
                Schemas.Add(schema);
            }
        }
    }
}
