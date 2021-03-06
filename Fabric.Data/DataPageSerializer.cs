﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;

namespace Fabric.Data {
    public class DataPageSerializer : JsonConverter {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DataPageSerializer" /> class.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        public DataPageSerializer(UnityContainer resolver) {
            Resolver = resolver;
        }

        internal UnityContainer Resolver { get; }

        /// <summary>
        ///     Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var jToken = JToken.FromObject(value);

            if (jToken.Type != JTokenType.Object) {
                jToken.WriteTo(writer);
                return;
            }

            writer.WriteStartObject();
            foreach (var property in value.GetType().GetProperties()) {
                var type = property.PropertyType;
                var propertyValue = property.GetValue(value);

                var nameToWrite = property.Name;
                var valuetoWrite = propertyValue;

                if (type == typeof(DataPageCollection)
                    && propertyValue != null) {
                    nameToWrite = "Children";
                    valuetoWrite = type.GetMethod("GetNames")?.Invoke(propertyValue, null);
                }

                if (string.Equals(nameToWrite, "pageData", StringComparison.InvariantCultureIgnoreCase) &&
                    propertyValue != null) {
                    valuetoWrite = JsonConvert.DeserializeObject((string) propertyValue);
                }

                writer.WritePropertyName(FirstLetterToLowerCase(nameToWrite));
                serializer.Serialize(writer, valuetoWrite);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        ///     Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        ///     The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var culture = CultureInfo.InvariantCulture;
            var instance = existingValue ?? Activator.CreateInstance(objectType, flags, null, null, culture);

            var jsonObject = JObject.Load(reader);
            var properties = jsonObject.Properties().ToList();

            foreach (var property in objectType.GetProperties()) {
                var type = property.PropertyType;
                object value;

                if (type == typeof(DataPageCollection)) {
                    var collection = new DataPageCollection(instance as DataPage, Resolver.Resolve<IChangeSetHelper>(),
                        Resolver.Resolve<IDatabaseHelper>());

                    var deserializedValue =
                        properties.FirstOrDefault(p =>
                                string.Equals(p.Name, property.Name, StringComparison.CurrentCultureIgnoreCase))
                            ?.Value.ToObject<Dictionary<string, List<string>>>();

                    collection.PopulateFromSerializer(deserializedValue);

                    value = collection;
                }
                else if (string.Equals(property.Name, "pageData", StringComparison.InvariantCultureIgnoreCase)) {
                    var prop = properties.FirstOrDefault(p =>
                        string.Equals(p.Name, property.Name, StringComparison.CurrentCultureIgnoreCase));

                    value = prop?.Value.ToString();
                }
                else {
                    var prop = properties.FirstOrDefault(p =>
                        string.Equals(p.Name, property.Name, StringComparison.CurrentCultureIgnoreCase));
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

        /// <summary>
        ///     Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType) {
            return typeof(DataPage).IsAssignableFrom(objectType);
        }

        public static string FirstLetterToLowerCase(string s) {
            if (string.IsNullOrEmpty(s)) {
                throw new ArgumentException("There is no first letter");
            }

            var a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }
    }
}
