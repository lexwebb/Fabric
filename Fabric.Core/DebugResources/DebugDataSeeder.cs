using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fabric.Data;
using Unity;

namespace Fabric.Core.DebugResources {
    internal static class DebugDataSeeder {
        internal static void SeedDebugData(FabricDatabase database) {
            var schemas = GetDebugSchemas();

            foreach (var schema in schemas) {
                database.Resolver.Resolve<ISchemaManager>().Add(schema.schemaName, schema.schemaRawJson);
            }

            CreateTestProjects(database);

            CreateTestDoors(database);

            CreateTestWorkflows(database);
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
        }

        private static void CreateTestDoors(FabricDatabase database) {
            const string schemaPath = "Fabric.Core.DebugResources.Pages.";
            var doorNames = new [] {"door1", "door2"};

            foreach(var doorName in doorNames) {
                var json = GetJsonFromResourcePath($"{schemaPath}{doorName}.json");
                database.Root.AddChild(doorName, "door", json);
            }
        }

        private static void CreateTestWorkflows(FabricDatabase database) {
            const string schemaPath = "Fabric.Core.DebugResources.Pages.";
            var workflowNames = new [] {"workflow1"};

            foreach(var workflowName in workflowNames) {
                var json = GetJsonFromResourcePath($"{schemaPath}{workflowName}.json");
                database.Root.AddChild(workflowName, "workflow", json);
            }
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
