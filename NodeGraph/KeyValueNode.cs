using System;

namespace Whaledevelop.NodeGraph
{
    [Serializable]
    public abstract class KeyValueNode<TNode>
        where TNode : BaseNode
    {
        public string Key { get; set; }
        public TNode Node { get; set; }
    }
}