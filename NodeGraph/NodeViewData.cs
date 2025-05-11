using System;
using UnityEngine;

namespace Whaledevelop.NodeGraph
{
    [Serializable]
    public abstract class NodeViewData
    {
        [SerializeField]
        private Rect _position;

        public Rect Position
        {
            get => _position;
            set => _position = value;
        }
    }

    [Serializable]
    public abstract class NodeViewData<TNode> : NodeViewData
        where TNode : BaseNode
    {
        [SerializeReference]
        private TNode _value;

        public TNode Value
        {
            get => _value;
            set => _value = value;
        }
    }
}