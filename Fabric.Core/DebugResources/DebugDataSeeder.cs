using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fabric.Data;

namespace Fabric.Core.DebugResources {
    internal static class DebugDataSeeder {
        internal static void SeedDebugData(FabricDatabase database) {
            var schemas = GetDebugSchemas();

            foreach (var schema in schemas) database.SchemaManager.Add(schema.schemaName, schema.schemaRawJson);

            CreateTestProjects(database);

            CreateTestCards(database);
        }

        private static IEnumerable<(string schemaName, string schemaRawJson)> GetDebugSchemas() {
            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            const string schemaPath = "Fabric.Core.DebugResources.Schemas.";
            var schemaResources = resources.Where(r => r.StartsWith(schemaPath));

            return (from schemaResource in schemaResources
                let schemaName = schemaResource.TrimStart(schemaPath).TrimEnd(".json")
                select (schemaName, GetJsonFromResourcePath(schemaResource))).ToList();
        }

        private static void CreateTestProjects(FabricDatabase database) {
            const string schemaPath = "Fabric.Core.DebugResources.Pages.";
            var projectNames = new[] {"project1", "project2"};

            foreach (var projectName in projectNames) {
                var json = GetJsonFromResourcePath($"{schemaPath}{projectName}.json");
                database.Root.AddChild(projectName, "project", json);
            }

            database.SaveChanges();
        }

        private static void CreateTestCards(FabricDatabase database) {
            const string schemaPath = "Fabric.Core.DebugResources.Pages.";
            var cardNames = new [] {"card1", "card2"};

            foreach(var cardName in cardNames) {
                var json = GetJsonFromResourcePath($"{schemaPath}{cardName}.json");
                database.Root.AddChild(cardName, "card", json);
            }

            database.SaveChanges();
        }

        private static string GetJsonFromResourcePath(string resourcePath) {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            using (var reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }
    }
}
