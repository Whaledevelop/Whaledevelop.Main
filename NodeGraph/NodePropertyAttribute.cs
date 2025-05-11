using System;

namespace Whaledevelop.NodeGraph
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NodePropertyAttribute : Attribute
    {
        public readonly string Name;
        public readonly NodeDirection Direction;
        public readonly NodePortCapacity Capacity;
        public readonly bool Optional;

        public NodePropertyAttribute(string name, NodeDirection direction, NodePortCapacity capacity = NodePortCapacity.Single, bool optional = false)
        {
            Name = name;
            Direction = direction;
            Capacity = capacity;
            Optional = optional;
        }
    }
}