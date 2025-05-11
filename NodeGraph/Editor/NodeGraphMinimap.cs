using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Whaledevelop.NodeGraph
{
    public sealed class NodeGraphMiniMap : MiniMap
    {
        private readonly string _nodeGraphWindowTypeName;

        private string MiniMapKey => $"{_nodeGraphWindowTypeName}.{nameof(NodeGraphMiniMap)}";
        private string VisibleKey => $"{MiniMapKey}.{nameof(Visible)}";
        private string PositionKey => $"{MiniMapKey}.{nameof(Position)}";
        private string PositionXKey => $"{PositionKey}.X";
        private string PositionYKey => $"{PositionKey}.Y";
        private string PositionWidthKey => $"{PositionKey}.Width";
        private string PositionHeightKey => $"{PositionKey}.Height";

        public bool Visible
        {
            get => EditorPrefs.GetBool(VisibleKey, true);
            set
            {
                visible = value;
                EditorPrefs.SetBool(VisibleKey, value);
            }
        }

        private Rect Position
        {
            get => new(
                EditorPrefs.GetFloat(PositionXKey, 10),
                EditorPrefs.GetFloat(PositionYKey, 28),
                EditorPrefs.GetFloat(PositionWidthKey, 150),
                EditorPrefs.GetFloat(PositionHeightKey, 150));
            set
            {
                EditorPrefs.SetFloat(PositionXKey, value.x);
                EditorPrefs.SetFloat(PositionYKey, value.y);
                EditorPrefs.SetFloat(PositionWidthKey, value.width);
                EditorPrefs.SetFloat(PositionHeightKey, value.height);
            }
        }

        public NodeGraphMiniMap(string nodeGraphWindowTypeName)
        {
            _nodeGraphWindowTypeName = nodeGraphWindowTypeName;
            visible = Visible;
            SetPosition(Position);
        }

        public override void UpdatePresenterPosition()
        {
            Position = layout;
        }
    }
}