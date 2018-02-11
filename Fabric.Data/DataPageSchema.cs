using Newtonsoft.Json.Schema;

namespace Fabric.Data {
    public struct DataPageSchema {
        public string SchemaName { get; set; }

        public string SchemaRaw {
            get => Schema.ToString();
            set => Schema = JSchema.Parse(value);
        }

        internal JSchema Schema { get; set; }

        public DataPageSchema(string schemaName, string schemaRawJson) {
            this.SchemaName = schemaName;
            var resolver = new JSchemaUrlResolver();
            this.Schema = JSchema.Parse(schemaRawJson, resolver);
        }
    }
}
