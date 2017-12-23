using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RelationshipAttribute : System.Attribute
    {
        internal Type ChildType { get; set; }

        public RelationshipAttribute(Type childType) {
            ChildType = childType;
        }
    }
}
