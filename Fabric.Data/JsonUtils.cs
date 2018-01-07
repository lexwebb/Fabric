using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fabric.Data {
    public static class JsonUtils {
        public static string RemoveProperty(string jsonInput, string propertyName, bool recursive) {
            if (!(JsonConvert.DeserializeObject(jsonInput) is JObject jObject))
                throw new Exception("Failed to create json object from input string.");

            jObject.Property(propertyName).Remove();

            if (recursive) {
                jObject = RemovePorpertyRecursive(jObject, propertyName);
            }

            return jObject.ToString();
        }

        private static JObject RemovePorpertyRecursive(JObject jObject, string propertyName) {
            if (jObject.Properties().Any(p => p.Name == propertyName))
                jObject.Property(propertyName).Remove();

            foreach (var property in jObject.Properties()) {
                switch (property.Value.Type) {
                    case JTokenType.Array:
                        var newJArray = new JArray();
                        foreach (var arrayObj in (JArray) property.Value) {
                            if (!(arrayObj is JObject obj)) {
                                newJArray.Add(arrayObj);
                            }
                            else {
                                newJArray.Add(RemovePorpertyRecursive(obj, propertyName));
                            }
                        }
                        property.Value = newJArray;
                        break;
                    case JTokenType.Object:
                        property.Value = RemovePorpertyRecursive((JObject)property.Value, propertyName);
                        break;
                }
            }

            return jObject;
        }

        public static string Uglify(string jsonInput) {
            if (!(JsonConvert.DeserializeObject(jsonInput) is JObject jObject))
                throw new Exception("Failed to create json object from input string.");

            return jObject.ToString(Formatting.None);
        }

        public static string Prettify(string jsonInput) {
            if (!(JsonConvert.DeserializeObject(jsonInput) is JObject jObject))
                throw new Exception("Failed to create json object from input string.");

            return jObject.ToString(Formatting.Indented);
        }
    }
}
