using System;
using System.Reflection;

namespace Whaledevelop.NodeGraph
{
    public readonly struct NodeDynamicPort
    {
        public readonly string PortName;
        public readonly NodeDirection Direction;
        public readonly NodePortCapacity PortCapacity;
        public readonly Type Type;
        public readonly PropertyInfo PropertyInfo;

        public NodeDynamicPort(string portName, NodeDirection direction, NodePortCapacity portCapacity, Type type, PropertyInfo propertyInfo)
        {
            PortName = portName;
            Direction = direction;
            PortCapacity = portCapacity;
            Type = type;
            PropertyInfo = propertyInfo;
        }
    }
}