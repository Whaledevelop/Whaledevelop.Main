using System.Collections.Generic;
using System.Linq;
using Whaledevelop.Extensions;
using UnityEditor;

namespace Whaledevelop.NodeGraph
{
    public partial class NodeGraphWindow<TNodeGraphView, TNodeView, TNodeGraphEditorData, TNodeViewData, TNode>
    {
        private void LoadGraph()
        {
            UnsubscribeGraph();

            _graphView.Reset();

            GraphData.Nodes.RemoveAll(IsNodeIncorrect);
            GraphData.Nodes.ForEach(AddNode);
            GraphData.Edges.RemoveAll(IsEdgeIncorrect);
            GraphData.Edges.ForEach(AddEdge);

            SubscribeGraph();
            OnGraphLoaded();

            void AddNode(TNodeViewData nodeViewData)
            {
                _graphView.AddNode(CreateNodeView(nodeViewData));
            }

            static bool IsNodeIncorrect(TNodeViewData node)
            {
                return node?.Value == null;
            }

            void AddEdge(NodeEdge nodeEdge)
            {
                _graphView.TryAddEdge(nodeEdge, out _);
            }

            bool IsEdgeIncorrect(NodeEdge nodeEdge)
            {
                if (nodeEdge == null)
                {
                    return true;
                }

                var node1 = GraphData.Nodes.FirstOrDefault(data => data.Value.Guid == nodeEdge.InputNodeGuid);
                var node2 = GraphData.Nodes.FirstOrDefault(data => data.Value.Guid == nodeEdge.OutputNodeGuid);

                if (node1 == null || node2 == null)
                {
                    return true;
                }

                return !HasNodePort(node1.Value, nodeEdge.InputPortName) || !HasNodePort(node2.Value, nodeEdge.OutputPortName);
            }

            // ReSharper disable once SuggestBaseTypeForParameter
            static bool HasNodePort(TNode node, string portName)
            {
                var (_, nodeMethodAttribute) = node
                    .GetMethodInfos()
                    .FirstOrDefault(tuple => tuple.nodeMethodAttribute.Name == portName);

                if (nodeMethodAttribute != null)
                {
                    return true;
                }

                var (_, nodePropertyAttribute) = node
                    .GetPropertyInfos()
                    .FirstOrDefault(tuple => tuple.nodePropertyAttribute.Name == portName);

                if (nodePropertyAttribute != null)
                {
                    return true;
                }

                if (node is not INodeDynamicPorts dynamicPorts)
                {
                    return false;
                }

                foreach (var dynamicPort in dynamicPorts.DynamicPorts)
                {
                    if (dynamicPort.PortName == portName)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void SaveGraph(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (AssetDatabase.Contains(GraphData))
            {
                var assetPath = AssetDatabase.GetAssetPath(GraphData);
                if (assetPath != path)
                {
                    AssetDatabase.CopyAsset(assetPath, path);
                    AssetDatabase.ForceReserializeAssets(GetAssetPaths());
                }
                else
                {
                    SaveGraphData();
                }
            }
            else
            {
                AssetDatabase.CreateAsset(GraphData, path);
            }

            AssetDatabase.Refresh();

            OnGraphSaved(path);

            IEnumerable<string> GetAssetPaths()
            {
                yield return path;
            }
        }
    }
}