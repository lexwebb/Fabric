using System;
using Newtonsoft.Json;

namespace Fabric.Data {
    public struct DataPageVersion {
        public Guid VersionGuid { get; }

        [JsonIgnore]
        public DataPage DataPage { get; }

        public string DataPagePath { get; }

        public DateTime CreateDateTime { get; }

        public int VersionNumber { get; }

        public DataPageVersion(Guid versionGuid, string dataPagePath, DateTime createDateTime, int versionNumber,
            DataPage dataPage) : this() {
            VersionGuid = versionGuid;
            DataPagePath = dataPagePath;
            CreateDateTime = createDateTime;
            VersionNumber = versionNumber;
            DataPage = dataPage;
        }
    }
}
