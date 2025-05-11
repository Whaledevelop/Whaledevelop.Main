using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Whaledevelop.NodeGraph
{
    public abstract class NodeView<TNodeViewData, TNode> : Node
        where TNodeViewData : NodeViewData<TNode>
        where TNode : BaseNode
    {
        private readonly List<Port> _dynamicPorts = new();

        public TNodeViewData Data { get; set; }

        public string Guid => Data.Value.Guid;

        protected abstract string PortViewStyleSheetName { get; }

        protected virtual string FirstTitle
        {
            get
            {
                var nodeAttribute = Data.Value.GetType().GetCustomAttribute<NodeAttribute>();

                if (nodeAttribute == null)
                {
                    return null;
                }

                var menuTitle = nodeAttribute.MenuTitle;
                var lastIndex = menuTitle.LastIndexOf('/');

                return lastIndex < 0
                    ? menuTitle
                    : menuTitle[(lastIndex + 1)..];
            }
        }

        protected virtual string MiddleTitle => string.Empty;

        protected virtual string LastTitle => $"\n {{{Guid[..13]}}}";

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnRefresh()
        {
        }

        protected virtual void OnAddToSelection()
        {
        }

        public void Initialize()
        {
            name = Data.Value.GetType().Name;

            ResetTitle();
            CreatePorts();
            OnInitialize();
        }

        protected void ResetTitle()
        {
            var newTitle = $"{MiddleTitle}{LastTitle}";

            title = string.IsNullOrWhiteSpace(newTitle) ? name : newTitle;
        }

        public void AddToSelection()
        {
            OnAddToSelection();
        }

        private void CreatePorts()
        {
            foreach (var (_, attribute) in Data.Value.GetMethodInfos())
            {
                CreatePort(attribute.Name, attribute.Direction, attribute.Capacity, attribute.MethodType);
            }

            foreach (var (propertyInfo, attribute) in Data.Value.GetPropertyInfos())
            {
                CreatePort(attribute.Name, attribute.Direction, attribute.Capacity, propertyInfo.PropertyType);
            }

            if (Data.Value is not INodeDynamicPorts dynamicPorts)
            {
                return;
            }

            foreach (var dynamicPort in dynamicPorts.DynamicPorts)
            {
                var port = CreatePort(dynamicPort.PortName, dynamicPort.Direction, dynamicPort.PortCapacity, dynamicPort.Type);
                _dynamicPorts.Add(port);
            }
        }

        private Port CreatePort(string portName, NodeDirection direction, NodePortCapacity portCapacity, Type type)
        {
            var port = InstantiatePort(Orientation.Horizontal, (Direction)direction, (Port.Capacity)portCapacity, type);

            port.portName = portName;

            if (type.IsGenericType)
            {
                port.AddToClassList("typeList" + type.GetMainType().Name);
            }

            var container = direction == NodeDirection.Input
                ? inputContainer
                : outputContainer;

            container.Add(port);
            port.styleSheets.Add(Resources.Load<StyleSheet>(PortViewStyleSheetName));

            return port;
        }

        private bool RefreshDynamicPorts(ICollection<(Port, string)> renamedPorts, ICollection<Port> removedPorts)
        {
            if (Data.Value is not INodeDynamicPorts dynamicPorts)
            {
                return false;
            }

            var isUpdated = false;
            var index = 0;

            foreach (var dynamicPort in dynamicPorts.DynamicPorts)
            {
                Port port;

                if (index >= _dynamicPorts.Count)
                {
                    port = CreatePort(dynamicPort.PortName, dynamicPort.Direction, dynamicPort.PortCapacity, dynamicPort.Type);

                    _dynamicPorts.Add(port);
                    isUpdated = true;
                }
                else
                {
                    port = _dynamicPorts[index];
                }

                if (port.direction != (Direction)dynamicPort.Direction ||
                    port.capacity != (Port.Capacity)dynamicPort.PortCapacity ||
                    port.portType != dynamicPort.Type)
                {
                    port = CreatePort(dynamicPort.PortName, dynamicPort.Direction, dynamicPort.PortCapacity, dynamicPort.Type);

                    _dynamicPorts.Insert(index, port);
                    isUpdated = true;
                }

                if (port.portName != dynamicPort.PortName)
                {
                    renamedPorts.Add((port, port.portName));
                    port.portName = dynamicPort.PortName;
                    isUpdated = true;
                }

                index++;
            }

            while (index < _dynamicPorts.Count)
            {
                removedPorts.Add(_dynamicPorts[index]);
                _dynamicPorts.RemoveAt(index);
                isUpdated = true;
            }

            return isUpdated;
        }

        public bool Refresh(ICollection<(Port, string)> renamedPorts, ICollection<Port> removedPorts)
        {
            var updated = RefreshDynamicPorts(renamedPorts, removedPorts);

            ResetTitle();
            OnRefresh();

            return updated;
        }
    }
}