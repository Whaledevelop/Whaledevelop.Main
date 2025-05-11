using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Whaledevelop.NodeGraph
{
    [Serializable]
    public abstract class NodeGraphData<TNode>
        where TNode : BaseNode
    {
        [SerializeReference]
        private TNode[] _nodes = Array.Empty<TNode>();

        [SerializeField]
        private NodeEdge[] _edges = Array.Empty<NodeEdge>();

        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public TNode[] Nodes => _nodes;

        public NodeEdge[] Edges => _edges;

        [Conditional("UNITY_EDITOR")]
        public void Set(IEnumerable<TNode> nodes, IEnumerable<NodeEdge> edges)
        {
            _nodes = nodes.ToArray();
            _edges = edges.ToArray();
        }

        [Conditional("UNITY_EDITOR")]
        public void Reset()
        {
            _nodes = Array.Empty<TNode>();
            _edges = Array.Empty<NodeEdge>();
        }
    }
}