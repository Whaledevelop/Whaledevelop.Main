using System;
using System.Collections.Generic;
using Whaledevelop.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

namespace Whaledevelop.NodeGraph
{
    public abstract class NodeGraphView<TNodeView, TNodeViewData, TNode> : GraphView
        where TNodeView : NodeView<TNodeViewData, TNode>
        where TNodeViewData : NodeViewData<TNode>
        where TNode : BaseNode
    {
        private readonly List<(Port, string)> _renamedPorts = new();
        private readonly List<Port> _removedPorts = new();

        public event Action<Edge> EdgeCreated;
        public event Action<Edge> EdgeRemoved;
        public event Action<Port, string> PortRenamed;
        public event Action<Node> NodeCreated;
        public event Action<Node> NodeRemoved;
        public event Action<Node, Vector2> NodeMoved;
        public event Action SelectionChanged;

        protected abstract string NodeGraphStyleSheetName { get; }

        private UQueryState<TNodeView> Nodes { get; }

        protected NodeGraphView()
        {
            SetBackgroundStyleSheet();
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();

            Insert(0, grid);

            grid.StretchToParentSize();

            graphViewChanged = OnGraphViewChanged;

            Nodes = contentViewContainer.Query<TNodeView>().Build();
        }

        private void SetBackgroundStyleSheet()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(NodeGraphStyleSheetName));
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                foreach (var element in graphViewChange.movedElements)
                {
                    if (element is Node node)
                    {
                        NodeMoved?.Invoke(node, graphViewChange.moveDelta);
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    EdgeCreated?.Invoke(edge);
                }
            }

            if (graphViewChange.elementsToRemove == null)
            {
                return graphViewChange;
            }

            foreach (var element in graphViewChange.elementsToRemove)
            {
                switch (element)
                {
                    case Node node:
                        NodeRemoved?.Invoke(node);
                        break;
                    case Edge edge:
                        EdgeRemoved?.Invoke(edge);
                        break;
                }
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(AddCompatiblePort);

            return compatiblePorts;

            void AddCompatiblePort(Port port)
            {
                if (IsProperDirectionAndNotSelf(port, startPort) &&
                    (AreMainTypesEqual(startPort, port) ||
                     IsDynamicToNext(startPort, port) ||
                     IsDynamicToNext(port, startPort)))
                {
                    compatiblePorts.Add(port);
                }
            }

            bool IsProperDirectionAndNotSelf(Port port1, Port port2)
            {
                return port1 != port2 && port1.node != port2.node && port1.direction != port2.direction;
            }

            bool AreMainTypesEqual(Port port1, Port port2)
            {
                return port1.portType.GetMainType() == port2.portType.GetMainType();
            }

            bool IsDynamicToNext(Port port1, Port port2)
            {
                return typeof(KeyValueNode<TNode>).IsAssignableFrom(port1.portType) && port2.portType == typeof(TNode);
            }
        }

        public override void AddToSelection(ISelectable selectable)
        {
            
            AddToSelection(selectable, true);
        }

        public void AddToSelection(ISelectable selectable, bool triggerEvent)
        {
            base.AddToSelection(selectable);

            if (selectable is TNodeView nodeView)
            {
                
                nodeView.AddToSelection();
            }

            if (triggerEvent)
            {
                SelectionChanged?.Invoke();
            }
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            SelectionChanged?.Invoke();
        }

        public override void ClearSelection()
        {
            ClearSelection(true);
        }

        public void ClearSelection(bool triggerEvent)
        {
            base.ClearSelection();

            if (triggerEvent)
            {
                SelectionChanged?.Invoke();
            }
        }

        public void AddNode(TNodeView nodeView)
        {
            AddElement(nodeView);
            NodeCreated?.Invoke(nodeView);
        }

        public bool TryAddEdge(NodeEdge nodeEdge, out Edge edge)
        {
            if (!TryFind<TNodeView>(nodeView => nodeView.Guid == nodeEdge.InputNodeGuid, out var nodeInput) ||
                !TryFind<TNodeView>(nodeView => nodeView.Guid == nodeEdge.OutputNodeGuid, out var nodeOutput) ||
                !TryFind<Port>(port => port.node == nodeInput && port.portName == nodeEdge.InputPortName, out var portInput) ||
                !TryFind<Port>(port => port.node == nodeOutput && port.portName == nodeEdge.OutputPortName, out var portOutput))
            {
                edge = null;
                return false;
            }

            edge = portOutput.ConnectTo(portInput);

            if (edge == null)
            {
                return false;
            }

            AddElement(edge);
            EdgeCreated?.Invoke(edge);

            return true;
        }

        private T Find<T>(Func<T, bool> predicate)
            where T : VisualElement
        {
            return contentViewContainer.Query<T>()
                .Where(predicate)
                .First();
        }

        public bool TryFind<T>(Func<T, bool> predicate, out T result)
            where T : VisualElement
        {
            result = Find(predicate);
            return result != null;
        }

        public void Reset()
        {
            edges.ForEach(RemoveElement);
            nodes.ForEach(RemoveElement);
        }

        public void Refresh()
        {
            Profiler.BeginSample("REFRESH");

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var node in Nodes)
            {
                if (!node.Refresh(_renamedPorts, _removedPorts))
                {
                    continue;
                }

                foreach (var (port, oldName) in _renamedPorts)
                {
                    PortRenamed?.Invoke(port, oldName);
                }

                _renamedPorts.Clear();

                foreach (var removedPort in _removedPorts)
                {
                    foreach (var edge in removedPort.connections)
                    {
                        EdgeRemoved?.Invoke(edge);
                    }

                    removedPort.Destroy();
                }

                _removedPorts.Clear();

                node.RefreshPorts();
            }

            Profiler.EndSample();
        }
    }
}