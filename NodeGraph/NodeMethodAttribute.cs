using System;

namespace Whaledevelop.NodeGraph
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NodeMethodAttribute : Attribute
    {
        public readonly string Name;
        public readonly NodeDirection Direction;
        public readonly NodePortCapacity Capacity;
        public readonly Type MethodType;
        public readonly bool Optional;

        public NodeMethodAttribute(string name, NodeDirection direction, NodePortCapacity capacity, Type methodType, bool optional = false)
        {
            Name = name;
            Direction = direction;
            Capacity = capacity;
            MethodType = methodType;
            Optional = optional;
        }
    }
}