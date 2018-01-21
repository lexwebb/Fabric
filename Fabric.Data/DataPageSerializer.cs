using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fabric.Data
{
    public class DataPageSerializer : JsonConverter
    {
        public bool PartialChildPageSerialization { get; set; }

        private readonly FabricDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPageSerializer" /> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="partialChildPageSerialization">if set to <c>true</c> [partial child page serialization].</param>
        public DataPageSerializer(FabricDatabase database, bool partialChildPageSerialization = true) {
            this.PartialChildPageSerialization = partialChildPageSerialization;
            this._database = database;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var jToken = JToken.FromObject(value);

            if (jToken.Type != JTokenType.Object)
            {
                jToken.WriteTo(writer);
                return;
            }

            if (PartialChildPageSerialization) {
                writer.WriteStartObject();
                foreach (var property in value.GetType().GetProperties()) {
                    var type = property.PropertyType;
                    var propertyValue = property.GetValue(value);

                    var nameToWrite = property.Name;
                    var valuetoWrite = propertyValue;

                    if (type == typeof(DataPageCollection)
                        && propertyValue != null) {
                        nameToWrite = "Children";
                        valuetoWrite = type.GetMethod("GetNames").Invoke(propertyValue, null);
                    }

                    writer.WritePropertyName(FirstLetterToLowerCase(nameToWrite));
                    serializer.Serialize(writer, valuetoWrite);
                }
                writer.WriteEndObject();
            }
            else {
                serializer.Serialize(writer, value);
            }
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (PartialChildPageSerialization) {
                var instance = existingValue ?? Activator.CreateInstance(objectType);

                var jsonObject = JObject.Load(reader);
                var properties = jsonObject.Properties().ToList();

                foreach (var property in objectType.GetProperties()) {
                    var type = property.PropertyType;
                    object value = null;

                    if (type == typeof(DataPageCollection)) {
                        var collection = new DataPageCollection(_database, instance as DataPage);
                        var deserializedValue =
                            properties.First(p => p.Name == property.Name).Value.ToObject<Dictionary<string, List<string>>>();
                        type.GetMethod("PopulateFromSerializer", BindingFlags.Instance | BindingFlags.NonPublic)
                            .Invoke(collection, new object[] {deserializedValue});
                        value = collection;
                    }
                    else {
                        var prop = properties.FirstOrDefault(p => p.Name == property.Name);
                        try {
                            value = prop?.ToObject(type);
                        }
                        catch {
                            value = null;
                        }
                    }

                    property.SetValue(instance, value);
                }

                return instance;
            }

            return serializer.Deserialize(reader, objectType);
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType) {
            return typeof(DataPage).IsAssignableFrom(objectType);
        }

        public static string FirstLetterToLowerCase(string s) {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");

            var a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }
    }
}
