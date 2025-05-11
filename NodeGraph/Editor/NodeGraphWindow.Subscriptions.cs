using System;
using System.Linq;
using Whaledevelop.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Whaledevelop.NodeGraph
{
    public partial class NodeGraphWindow<TNodeGraphView, TNodeView, TNodeGraphEditorData, TNodeViewData, TNode>
    {
        private void UnsubscribeGraph()
        {
            _graphView.EdgeCreated -= OnEdgeCreated;
            _graphView.EdgeRemoved -= OnEdgeRemoved;
            _graphView.PortRenamed -= OnPortRenamed;
            _graphView.NodeCreated -= OnNodeCreated;
            _graphView.NodeRemoved -= OnNodeRemoved;
            _graphView.NodeMoved -= OnNodeMoved;
            _graphView.SelectionChanged -= OnGraphSelectionChanged;
        }

        private void SubscribeGraph()
        {
            _graphView.EdgeCreated += OnEdgeCreated;
            _graphView.EdgeRemoved += OnEdgeRemoved;
            _graphView.PortRenamed += OnPortRenamed;
            _graphView.NodeCreated += OnNodeCreated;
            _graphView.NodeRemoved += OnNodeRemoved;
            _graphView.NodeMoved += OnNodeMoved;
            _graphView.SelectionChanged += OnGraphSelectionChanged;
        }

        private void OnEdgeCreated(Edge edge)
        {
            if (edge.input.node is not TNodeView nodeInput || edge.output.node is not TNodeView nodeOutput)
            {
                return;
            }

            RecordObjectForUndo($"Create {nameof(NodeEdge)} {{{nodeOutput.Guid[..13]}}}-{{{nodeInput.Guid[..13]}}}");

            GraphData.Edges.Add(new NodeEdge
            {
                InputNodeGuid = nodeInput.Guid,
                InputPortName = edge.input.portName,
                OutputNodeGuid = nodeOutput.Guid,
                OutputPortName = edge.output.portName
            });
        }

        private void OnEdgeRemoved(Edge edge)
        {
            if (edge.input.node is not TNodeView nodeInput || edge.output.node is not TNodeView nodeOutput)
            {
                return;
            }

            if (!GraphData.Edges.TryGetFirst(item =>
                    item.InputNodeGuid == nodeInput.Guid &&
                    item.OutputNodeGuid == nodeOutput.Guid &&
                    item.InputPortName == edge.input.portName &&
                    item.OutputPortName == edge.output.portName, out var nodeEdge))
            {
                return;
            }

            RecordObjectForUndo($"Remove {nameof(NodeEdge)} {{{nodeOutput.Guid[..13]}}}-{{{nodeInput.Guid[..13]}}}");
            GraphData.Edges.Remove(nodeEdge);
        }

        private void OnPortRenamed(Port port, string oldName)
        {
            if (port.node is not TNodeView node)
            {
                return;
            }

            RecordObjectForUndo($"Renamed {nameof(Port)} {oldName} to {port.portName}");

            var input = port.direction == Direction.Input;

            foreach (var edge in port.connections)
            {
                var otherPort = input ? edge.output : edge.input;

                if (otherPort.node is not TNodeView otherNode)
                {
                    continue;
                }

                var inputNodeGuid = input ? node.Guid : otherNode.Guid;
                var inputPortName = input ? oldName : otherPort.portName;
                var outputNodeGuid = input ? otherNode.Guid : node.Guid;
                var outputPortName = input ? otherPort.portName : oldName;

                if (!GraphData.Edges.TryGetFirst(item =>
                        item.InputNodeGuid == inputNodeGuid &&
                        item.OutputNodeGuid == outputNodeGuid &&
                        item.InputPortName == inputPortName &&
                        item.OutputPortName == outputPortName, out var nodeEdge))
                {
                    continue;
                }

                if (input)
                {
                    nodeEdge.InputPortName = port.portName;
                }
                else
                {
                    nodeEdge.OutputPortName = port.portName;
                }
            }
        }

        private void OnNodeCreated(Node node)
        {
            if (node is not TNodeView nodeView)
            {
                return;
            }

            RecordObjectForUndo($"Create {nodeView.name} {{{nodeView.Guid[..13]}}}");
            GraphData.Nodes.Add(nodeView.Data);
        }

        private void OnNodeRemoved(Node node)
        {
            if (node is not TNodeView nodeView)
            {
                return;
            }

            RecordObjectForUndo($"Remove {nodeView.name} {{{nodeView.Guid[..13]}}}");
            GraphData.Edges.RemoveAll(edge => edge.InputNodeGuid == nodeView.Guid || edge.OutputNodeGuid == nodeView.Guid);
            GraphData.Nodes.Remove(nodeView.Data);
            OnGraphSelectionChanged();
        }

        private void OnNodeMoved(Node node, Vector2 delta)
        {
            if (node is not TNodeView nodeView)
            {
                return;
            }

            RecordObjectForUndo($"Move {nodeView.name} {{{nodeView.Guid[..13]}}}");

            var nodePosition = nodeView.Data.Position;
            nodePosition.position += delta;
            nodeView.Data.Position = nodePosition;
        }

        private void OnGraphSelectionChanged()
        {
            EditorApplication.delayCall += ChangeGraphSelection;
        }

        private void OnNodeCreationRequest(NodeCreationContext nodeCreationContext)
        {
            SearchWindow.Open(new SearchWindowContext(nodeCreationContext.screenMousePosition), _searchProvider);
        }

        private void OnCreateNode(Type nodeType, Vector2 mousePosition)
        {
            
            var windowMousePosition = mousePosition != Vector2.zero
                ? _graphView.ChangeCoordinatesTo(_graphView.parent, mousePosition - position.position)
                : position.center;
            var nodeViewData = new TNodeViewData
            {
                Position = new Rect(_graphView.contentViewContainer.WorldToLocal(windowMousePosition), new Vector2(400f, 150f)),
                Value = (TNode)Activator.CreateInstance(nodeType)
            };
            var nodeView = CreateNodeView(nodeViewData);

            _graphView.AddNode(nodeView);
            nodeView.Select(_graphView, false);
        }
    }
}