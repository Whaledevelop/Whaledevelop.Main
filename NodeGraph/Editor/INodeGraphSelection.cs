using System;
using System.Collections.Generic;

namespace Whaledevelop.NodeGraph
{
    public interface INodeGraphSelection
    {
        event Action<bool> SelectionChanged;

        IReadOnlyList<string> NodeGuids { get; }

        IReadOnlyList<NodeEdge> Edges { get; }

        bool ShowFullTree { get; set; }

        public void SetSelection(string undoActionName, IReadOnlyList<string> nodeGuids, IReadOnlyList<NodeEdge> edges);

        public void ResetSelection(string undoActionName);

        void InvokeSelectionChanged(bool shouldExpand = false);
    }
}