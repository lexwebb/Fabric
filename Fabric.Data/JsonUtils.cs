using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fabric.Data {
    public static class JsonUtils {
        /// <summary>
        ///     Removes the specified property fomr the given JSON string.
        /// </summary>
        /// <param name="jsonInput">The json input.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        /// <exception cref="Exception">Failed to create json object from input string.</exception>
        public static string RemoveProperty(string jsonInput, string propertyName, bool recursive) {
            if (!(JsonConvert.DeserializeObject(jsonInput) is JObject jObject)) {
                throw new Exception("Failed to create json object from input string.");
            }

            jObject.Property(propertyName)?.Remove();

            if (recursive) {
                jObject = RemovePorpertyRecursive(jObject, propertyName);
            }

            return jObject.ToString();
        }

        /// <summary>
        ///     Removes the porperty recursive.
        /// </summary>
        /// <param name="jObject">The j object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private static JObject RemovePorpertyRecursive(JObject jObject, string propertyName) {
            if (jObject.Properties().Any(p => p.Name == propertyName)) {
                jObject.Property(propertyName).Remove();
            }

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
                        property.Value = RemovePorpertyRecursive((JObject) property.Value, propertyName);
                        break;
                }
            }

            return jObject;
        }

        /// <summary>
        ///     Uglifies the specified json input.
        /// </summary>
        /// <param name="jsonInput">The json input.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Failed to create json object from input string.</exception>
        public static string Uglify(string jsonInput) {
            if (!(JsonConvert.DeserializeObject(jsonInput) is JObject jObject)) {
                throw new Exception("Failed to create json object from input string.");
            }

            return jObject.ToString(Formatting.None);
        }

        /// <summary>
        ///     Prettifies the specified json input.
        /// </summary>
        /// <param name="jsonInput">The json input.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Failed to create json object from input string.</exception>
        public static string Prettify(string jsonInput) {
            if (!(JsonConvert.DeserializeObject(jsonInput) is JObject jObject)) {
                throw new Exception("Failed to create json object from input string.");
            }

            return jObject.ToString(Formatting.Indented);
        }
    }
}