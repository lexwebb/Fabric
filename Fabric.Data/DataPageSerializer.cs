using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fabric.Data
{
    public class DataPageSerializer : JsonConverter
    {
        public bool PartialChildPageSerialization { get; set; }

        public DataPageSerializer(bool partialChildPageSerialization = true) {
            PartialChildPageSerialization = partialChildPageSerialization;
        }

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

                    writer.WritePropertyName(property.Name);
                    var valuetoWrite = propertyValue;

                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ChildPageCollection<>)) {
                        valuetoWrite = type.GetMethod("GetNames").Invoke(propertyValue, null);
                    }

                    serializer.Serialize(writer, valuetoWrite);
                }
                writer.WriteEndObject();
            }
            else {
                serializer.Serialize(writer, value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (PartialChildPageSerialization) {
                var instance = existingValue ?? Activator.CreateInstance(objectType);

                var jsonObject = JObject.Load(reader);
                var properties = jsonObject.Properties().ToList();

                foreach (var property in objectType.GetProperties()) {
                    var type = property.PropertyType;
                    object value = null;

                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ChildPageCollection<>)) {
                        var collection = Activator.CreateInstance(type);
                        var deserializedValue =
                            properties.First(p => p.Name == property.Name).Value.ToObject<string[]>();
                        type.GetMethod("PopulateFromSerializer", BindingFlags.Instance | BindingFlags.NonPublic)
                            .Invoke(collection, new object[] {deserializedValue});
                        value = collection;
                    }
                    else {
                        value = properties.First(p => p.Name == property.Name).ToObject(type);
                    }

                    property.SetValue(instance, value);
                }

                return instance;
            }

            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType) {
            return typeof(DataPage).IsAssignableFrom(objectType);
        }
    }
}
