using System;

namespace Fabric.Data {
    public struct DataPageVersion {
        public Guid VersionGuid { get; }
        public string DataPagePath { get; }
        public DateTime CreateDateTime { get; }
        public int VersionNumber { get; }

        public DataPageVersion(Guid versionGuid, string dataPagePath, DateTime createDateTime, int versionNumber) : this() {
            VersionGuid = versionGuid;
            DataPagePath = dataPagePath;
            CreateDateTime = createDateTime;
            VersionNumber = versionNumber;
        }
    }
}
