using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Whaledevelop.NodeGraph
{
    public abstract class NodeGraphSelection<T> : ScriptableSingleton<T>, INodeGraphSelection
        where T : ScriptableObject
    {
        [SerializeField]
        private List<string> _nodeGuids = new();

        [SerializeField]
        private List<NodeEdge> _edges = new();

        [NonSerialized]
        private bool _showFullTree;

        private event Action<bool> SelectionChanged;

        private void InvokeSelectionChanged(bool shouldExpand)
        {
            _showFullTree = false;
            SelectionChanged?.Invoke(shouldExpand);
        }

        #region INodeGraphSelection

        event Action<bool> INodeGraphSelection.SelectionChanged
        {
            add => SelectionChanged += value;
            remove => SelectionChanged -= value;
        }

        IReadOnlyList<string> INodeGraphSelection.NodeGuids => _nodeGuids;

        IReadOnlyList<NodeEdge> INodeGraphSelection.Edges => _edges;

        bool INodeGraphSelection.ShowFullTree
        {
            get => _showFullTree;
            set => _showFullTree = value;
        }

        void INodeGraphSelection.SetSelection(string undoActionName, IReadOnlyList<string> nodeGuids,
            IReadOnlyList<NodeEdge> edges)
        {
            if (nodeGuids.Count == _nodeGuids.Count && nodeGuids.All(_nodeGuids.Contains) &&
                edges.Count == _edges.Count && edges.All(_edges.Contains))
            {
                return;
            }

            Undo.RegisterCompleteObjectUndo(this, undoActionName);

            _nodeGuids.Clear();
            _nodeGuids.AddRange(nodeGuids);
            _edges.Clear();
            _edges.AddRange(edges);

            InvokeSelectionChanged(true);
        }

        void INodeGraphSelection.ResetSelection(string undoActionName)
        {
            if (_nodeGuids.Count == 0 && _edges.Count == 0)
            {
                return;
            }

            Undo.RegisterCompleteObjectUndo(this, undoActionName);

            _nodeGuids.Clear();
            _edges.Clear();

            InvokeSelectionChanged(true);
        }

        void INodeGraphSelection.InvokeSelectionChanged(bool shouldExpand)
        {
            InvokeSelectionChanged(shouldExpand);
        }

        #endregion
    }
}