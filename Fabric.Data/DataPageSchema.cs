using Newtonsoft.Json.Schema;

namespace Fabric.Data {
    public struct DataPageSchema {
        public string SchemaName { get; set; }

        internal string SchemaRaw { get; set; }

        internal JSchema Schema { get; set; }

        public DataPageSchema(string schemaName, string schemaRawJson) {
            this.SchemaName = schemaName;
            this.SchemaRaw = schemaRawJson;
            this.Schema = JSchema.Parse(SchemaRaw);
        }
    }
}
