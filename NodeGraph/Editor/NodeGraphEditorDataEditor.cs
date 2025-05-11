using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Whaledevelop.NodeGraph
{
    [CustomEditor(typeof(NodeGraphEditorData), true)]
    public class NodeGraphEditorDataEditor : OdinEditor
    {
        private const int DEFAULT_EXPAND_LEVEL = 3;

        private readonly Dictionary<string, InspectorProperty> _targetProperties = new();

        private NodeGraphEditorData _graphData;
        private bool _needReset;
        private bool _shouldExpand;

        private bool Initialized => _graphData != null;

        private INodeGraphSelection NodeGraphSelection => _graphData.NodeGraphSelection;

        private bool ShowFullTree
        {
            get => NodeGraphSelection.ShowFullTree;
            set => NodeGraphSelection.ShowFullTree = value;
        }

        private IReadOnlyList<string> SelectedNodeGuids => NodeGraphSelection.NodeGuids;

        protected override void OnEnable()
        {
            base.OnEnable();

            _graphData = (NodeGraphEditorData)target;
            _needReset = true;
            ResetTargetProperties();
            NodeGraphSelection.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDisable()
        {
            NodeGraphSelection.SelectionChanged -= OnSelectionChanged;
            _graphData = null;
            _needReset = false;
            _targetProperties.Clear();

            base.OnDisable();
        }

        private void OnSelectionChanged(bool shouldExpand)
        {
            _needReset = true;
            _shouldExpand = shouldExpand;

            Repaint();
        }

        protected override void DrawTree()
        {
            if (Initialized && SelectedNodeGuids.Count > 0)
            {
                var buttonText = ShowFullTree ? "Show Node View" : "Show Data View";

                if (GUILayout.Button(buttonText))
                {
                    ShowFullTree = !ShowFullTree;
                }

                if (ShowFullTree)
                {
                    base.DrawTree();
                }
                else
                {
                    Tree.DrawMonoScriptObjectField = false;
                    Tree.BeginDraw(true);

                    ResetTargetProperties();

                    var shouldExpand = false;
                    var shouldCollapse = false;

                    if (_targetProperties.Count > 1)
                    {
                        shouldExpand = GUILayout.Button("Expand Root Properties");
                        shouldCollapse = GUILayout.Button("Collapse Root Properties");
                    }

                    foreach (var selectedNodeGuid in SelectedNodeGuids)
                    {
                        if (!_targetProperties.TryGetValue(selectedNodeGuid, out var targetProperty))
                        {
                            continue;
                        }

                        if (shouldExpand)
                        {
                            ExpandInspectorProperty(targetProperty, true);
                        }
                        else if (shouldCollapse)
                        {
                            ExpandInspectorProperty(targetProperty, false);
                        }

                        targetProperty.Draw();
                    }

                    Tree.EndDraw();
                }
            }
            else
            {
                base.DrawTree();
            }
        }

        private static void ExpandInspectorProperty(InspectorProperty inspectorProperty, bool expand, int level = DEFAULT_EXPAND_LEVEL)
        {
            inspectorProperty.State.Expanded = expand;

            if (--level <= 0)
            {
                return;
            }

            foreach (var inspectorPropertyChild in inspectorProperty.Children)
            {
                ExpandInspectorProperty(inspectorPropertyChild, expand, level);
            }
        }

        private void ResetTargetProperties()
        {
            if (!Initialized || !_needReset)
            {
                return;
            }

            var shouldExpand = _shouldExpand;
            var expand = SelectedNodeGuids.Count == 1;

            _needReset = false;
            _shouldExpand = false;
            _targetProperties.Clear();

            Tree.RootProperty.RefreshSetup();

            foreach (var inspectorProperty in Tree.EnumerateTree())
            {
                var valueEntry = inspectorProperty.ValueEntry;

                if (valueEntry == null || !typeof(BaseNode).IsAssignableFrom(valueEntry.TypeOfValue))
                {
                    continue;
                }

                var node = (BaseNode)valueEntry.WeakSmartValue;

                inspectorProperty.Label.text = valueEntry.TypeOfValue.Name;

                if (shouldExpand)
                {
                    ExpandInspectorProperty(inspectorProperty, expand);
                }

                _targetProperties.Add(node.Guid, inspectorProperty);
            }
        }
    }
}