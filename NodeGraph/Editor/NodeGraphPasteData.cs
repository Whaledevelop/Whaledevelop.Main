using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Whaledevelop.NodeGraph
{
    [Serializable]
    public class NodeGraphPasteData<TNodeView, TNodeViewData, TNode>
        where TNodeView : NodeView<TNodeViewData, TNode>
        where TNodeViewData : NodeViewData<TNode>
        where TNode : BaseNode
    {
        [SerializeField]
        private List<TNodeViewData> _nodes = new();

        [SerializeField]
        private List<NodeEdge> _edges = new();

        public IReadOnlyList<TNodeViewData> Nodes => _nodes;
        public IReadOnlyList<NodeEdge> Edges => _edges;

        public NodeGraphPasteData()
        {
        }

        public NodeGraphPasteData(IEnumerable<GraphElement> elements)
        {
            foreach (var element in elements)
            {
                switch (element)
                {
                    case TNodeView nodeView:
                        _nodes.Add(nodeView.Data);
                        break;
                    case Edge edge when edge.input.node is TNodeView inputNode && edge.output.node is TNodeView outputNode:
                        _edges.Add(new NodeEdge
                        {
                            InputNodeGuid = inputNode.Guid,
                            InputPortName = edge.input.portName,
                            OutputNodeGuid = outputNode.Guid,
                            OutputPortName = edge.output.portName
                        });
                        break;
                }
            }
        }
    }
}