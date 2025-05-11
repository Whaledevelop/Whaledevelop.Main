using System.Collections.Generic;

namespace Whaledevelop.NodeGraph
{
    public interface INodeDynamicPorts
    {
        IEnumerable<NodeDynamicPort> DynamicPorts { get; }
    }
}