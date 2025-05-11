using System;

namespace Whaledevelop.NodeGraph
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeEditorAttribute : Attribute
    {
        public readonly Type NodeType;

        public NodeEditorAttribute(Type nodeType)
        {
            NodeType = nodeType;
        }
    }
}