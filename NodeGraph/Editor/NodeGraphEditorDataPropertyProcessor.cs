using System.Collections.Generic;
using System.Linq;
using Whaledevelop.Extensions;
using Sirenix.OdinInspector.Editor;

namespace Whaledevelop.NodeGraph
{
    // ReSharper disable once UnusedType.Global
    public class NodeGraphEditorDataPropertyProcessor<TNodeGraphEditorData, TNodeViewData, TNode> : OdinPropertyProcessor<TNodeGraphEditorData>
        where TNodeGraphEditorData : NodeGraphEditorData<TNodeViewData, TNode>
        where TNodeViewData : NodeViewData<TNode>, new()
        where TNode : BaseNode
    {
        private readonly List<TNodeViewData> _nodes = new();
        private readonly List<NodeEdge> _edges = new();

        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            propertyInfos.AddValue<TNodeGraphEditorData, IList<TNodeViewData>>(nameof(_nodes), GetNodes, SetNodes);
            propertyInfos.AddValue<TNodeGraphEditorData, IList<NodeEdge>>(nameof(_edges), GetEdges, SetEdges);
        }

        private IList<TNodeViewData> GetNodes(ref TNodeGraphEditorData data)
        {
            var nodes = data.Nodes;
            var selectedNodeGuids = data.NodeGraphSelection.NodeGuids;
            var showFullTree = data.NodeGraphSelection.ShowFullTree || selectedNodeGuids.Count == 0;

            // ReSharper disable once InvertIf
            if (NeedUpdate())
            {
                _nodes.Clear();
                _nodes.AddRange(showFullTree ? nodes : nodes.Where(NodeIsSelected));
            }

            return _nodes;

            bool NeedUpdate()
            {
                return (showFullTree && (_nodes.Count != nodes.Count || _nodes.Exists(NodeDoesNotExist))) ||
                       (!showFullTree && (_nodes.Count != selectedNodeGuids.Count || _nodes.Exists(NodeIsNotSelected)));
            }

            bool NodeExists(TNodeViewData node)
            {
                return nodes.Exists(item => item.Value.Guid == node.Value.Guid);
            }

            bool NodeDoesNotExist(TNodeViewData node)
            {
                return !NodeExists(node);
            }

            bool NodeIsSelected(TNodeViewData node)
            {
                return selectedNodeGuids.Contains(node.Value.Guid);
            }

            bool NodeIsNotSelected(TNodeViewData node)
            {
                return !NodeIsSelected(node);
            }
        }

        private static void SetNodes(ref TNodeGraphEditorData data, IList<TNodeViewData> nodes)
        {
            data.Nodes.Clear();
            nodes?.ToList(data.Nodes);
        }

        private IList<NodeEdge> GetEdges(ref TNodeGraphEditorData data)
        {
            if (data.NodeGraphSelection.ShowFullTree || data.NodeGraphSelection.NodeGuids.Count == 0)
            {
                if (_edges.Count == 0)
                {
                    data.Edges.ToList(_edges);
                }
            }
            else
            {
                if (_edges.Count > 0)
                {
                    _edges.Clear();
                }
            }

            return _edges;
        }

        private static void SetEdges(ref TNodeGraphEditorData data, IList<NodeEdge> edges)
        {
            data.Edges.Clear();
            edges?.ToList(data.Edges);
        }
    }
}