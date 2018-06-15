using System;

namespace Fabric.Data {
    [AttributeUsage(AttributeTargets.Property)]
    public class RelationshipAttribute : Attribute {
        public RelationshipAttribute(Type childType) {
            ChildType = childType;
        }

        internal Type ChildType { get; set; }
    }
}