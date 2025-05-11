using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Whaledevelop.Extensions;
using Whaledevelop.Scopes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Whaledevelop.NodeGraph
{
    public abstract partial class NodeGraphWindow<TNodeGraphView, TNodeView, TNodeGraphEditorData, TNodeViewData, TNode> : EditorWindow
        where TNodeGraphView : NodeGraphView<TNodeView, TNodeViewData, TNode>, new()
        where TNodeView : NodeView<TNodeViewData, TNode>
        where TNodeGraphEditorData : NodeGraphEditorData<TNodeViewData, TNode>
        where TNodeViewData : NodeViewData<TNode>, new()
        where TNode : BaseNode
    {
        private const int UPDATE_COUNT_TO_FRAME_GRAPH_VIEW = 3;

        private TNodeGraphView _graphView;
        private NodeSearchWindowProvider _searchProvider;
        private NodeGraphMiniMap _miniMap;
        private Toolbar _toolbar;
        private ToolbarButton _titleToolbarButton;

        private TNodeGraphEditorData _graphData;

        private bool _graphDataIsDirty;
        private int _updateCountToFrameGraphView;

        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        protected TNodeGraphEditorData GraphData => _graphData;

        private bool IsLoaded => GraphData != null;

        protected abstract string DirectoryPath { get; }

        protected abstract string NewFileName { get; }

        protected string TempGraphPath => $"{DirectoryPath}TempGraph.asset";

        protected bool IsDefault => AssetDatabase.GetAssetPath(GraphData) == TempGraphPath;

        private string LastGraphPathKey => $"{GetType().Name}.{nameof(LastGraphPath)}";

        private string LastGraphPath
        {
            get => EditorPrefs.GetString(LastGraphPathKey, TempGraphPath);
            set => EditorPrefs.SetString(LastGraphPathKey, value);
        }

        private INodeGraphSelection NodeGraphSelection => GraphData.NodeGraphSelection;

        protected abstract TNodeView CreateNodeView(TNodeViewData nodeViewData);

        protected abstract NodeSearchWindowProvider CreateNodeSearchWindowProvider();

        private Vector2 LastMousePosition { get; set; }

        protected virtual void OnToolbarLeftInitialized()
        {
        }

        protected virtual void OnBeforeToolbarRightInitialized()
        {
        }

        protected virtual void OnWindowEnabled()
        {
        }

        protected virtual void OnGraphLoaded()
        {
        }

        protected virtual void OnGraphSaved(string path)
        {
        }

        protected ToolbarButton AddToolbarButton(string text, Action clickCallback)
        {
            var toolbarButton = new ToolbarButton(clickCallback)
            {
                text = text
            };

            _toolbar.Add(toolbarButton);

            return toolbarButton;
        }

        private void AddToolbarToggle(string text, bool value, EventCallback<ChangeEvent<bool>> valueChangedCallback)
        {
            var toggle = new ToolbarToggle
            {
                text = text,
                value = value
            };

            toggle.RegisterValueChangedCallback(valueChangedCallback);

            _toolbar.Add(toggle);
        }

        private void AddToolbarSpacer(bool flex)
        {
            _toolbar.Add(new ToolbarSpacer
            {
                flex = flex
            });
        }

        private void StartGraphAtPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var graphData = AssetDatabase.LoadAssetAtPath<TNodeGraphEditorData>(path);

            if (graphData != null)
            {
                StartGraph(graphData);
            }
        }

        private void StartLastGraph()
        {
            if (IsLoaded || AssetDatabase.IsAssetImportWorkerProcess())
            {
                return;
            }

            var graphData = AssetDatabase.LoadAssetAtPath<TNodeGraphEditorData>(LastGraphPath) ?? CreateGraphData();

            StartGraph(graphData);
        }

        private void SaveGraphData()
        {
            if (!IsLoaded)
            {
                return;
            }

            GraphData.ForceSaveAsset();
            GraphData.ForceReserializeAsset();
        }

        public void ShowGraph(TNodeGraphEditorData graphData)
        {
            Show();
            StartGraph(graphData);
        }

        private void StartGraph(TNodeGraphEditorData graphData)
        {
            ResetSelection();
            SaveGraphData();

            _graphData = graphData;

            LoadGraph();
            InitializeToolbar();
            DelayedFrameGraphView();

            LastGraphPath = AssetDatabase.GetAssetPath(GraphData);
        }

        private TNodeGraphEditorData CreateGraphData()
        {
            var graphData = CreateInstance<TNodeGraphEditorData>();

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            AssetDatabase.CreateAsset(graphData, TempGraphPath);
            AssetDatabase.Refresh();

            return graphData;
        }

        private void CreateGraph()
        {
            SaveGraphData();

            var graphData = CreateGraphData();

            StartGraph(graphData);
        }

        protected void RecordObjectForUndo(string actionName)
        {
            Undo.RecordObject(GraphData, $"{actionName} {GraphData.name}");
        }

        private void ChangeGraphSelection()
        {
            
            using (ListScope<string>.Create(out var nodeGuids))
            using (ListScope<NodeEdge>.Create(out var edges))
            {
                _graphView.selection
                    .OfType<TNodeView>()
                    .Select(item => item.Guid)
                    .ToList(nodeGuids);
                _graphView.selection
                    .OfType<Edge>()
                    .Where(IsNodeEdge)
                    .Select(CreateNodeEdge)
                    .ToList(edges);

                NodeGraphSelection.SetSelection($"Selection changed {GraphData.name}", nodeGuids, edges);

                
                if (nodeGuids.Count > 0)
                {
                    Selection.activeObject = GraphData;
                }
            }

            bool IsNodeEdge(Edge edge)
            {
                return edge.input.node is TNodeView && edge.output.node is TNodeView;
            }

            NodeEdge CreateNodeEdge(Edge edge)
            {
                var nodeInput = (TNodeView)edge.input.node;
                var nodeOutput = (TNodeView)edge.output.node;

                return new NodeEdge
                {
                    InputNodeGuid = nodeInput.Guid,
                    InputPortName = edge.input.portName,
                    OutputNodeGuid = nodeOutput.Guid,
                    OutputPortName = edge.output.portName
                };
            }
        }

        private void RestoreSelection()
        {
            EditorApplication.delayCall += RestoreSelectionInternal;

            void RestoreSelectionInternal()
            {
                foreach (var guid in NodeGraphSelection.NodeGuids)
                {
                    if (_graphView.TryFind<TNodeView>(item => item.Guid == guid, out var nodeView))
                    {
                        _graphView.AddToSelection(nodeView, false);
                    }
                }

                foreach (var nodeEdge in NodeGraphSelection.Edges)
                {
                    if (_graphView.TryFind<Edge>(item =>
                                item.input.node is TNodeView nodeInput &&
                                item.output.node is TNodeView nodeOutput &&
                                nodeEdge.InputNodeGuid == nodeInput.Guid &&
                                nodeEdge.OutputNodeGuid == nodeOutput.Guid &&
                                nodeEdge.InputPortName == item.input.portName &&
                                nodeEdge.OutputPortName == item.output.portName,
                            out var edge))
                    {
                        _graphView.AddToSelection(edge, false);
                    }
                }

                NodeGraphSelection.InvokeSelectionChanged();

                if (_graphView.selection.Any(item => item is TNodeView))
                {
                    Selection.activeObject = GraphData;
                }
            }
        }

        private void ResetSelection()
        {
            if (IsLoaded && (NodeGraphSelection.NodeGuids.Count > 0 || NodeGraphSelection.Edges.Count > 0))
            {
                NodeGraphSelection.ResetSelection($"Selection reset {GraphData.name}");
            }
        }

        private void DelayedFrameGraphView()
        {
            _updateCountToFrameGraphView = UPDATE_COUNT_TO_FRAME_GRAPH_VIEW;
        }

        #region Initialization

        private void InitializeGraphView()
        {
            ReleaseGraphView();

            _graphView = new TNodeGraphView();
            _graphView.StretchToParentSize();
            _graphView.nodeCreationRequest = OnNodeCreationRequest;
            _graphView.serializeGraphElements = OnSerializeGraphElements;
            _graphView.canPasteSerializedData = OnCanPasteSerializedData;
            _graphView.unserializeAndPaste = OnUnserializeAndPaste;

            rootVisualElement.Add(_graphView);

            _miniMap = new NodeGraphMiniMap(GetType().Name);
            _graphView.Add(_miniMap);
        }

        private void ReleaseGraphView()
        {
            if (_graphView != null)
            {
                rootVisualElement.Remove(_graphView);
            }

            _graphView = null;
            _miniMap = null;
        }

        private void InitializeToolbar()
        {
            ReleaseToolbar();

            _toolbar = new Toolbar();
            rootVisualElement.Add(_toolbar);

            AddToolbarButton("Create new", OnCreateNewButtonClick);
            AddToolbarButton("Load", OnLoadButtonClick);

            if (!IsDefault)
            {
                AddToolbarButton("Save", OnSaveButtonClick);
            }

            AddToolbarButton("Save as", OnSaveAsButtonClick);
            _titleToolbarButton = AddToolbarButton(GraphData.name, OnTitleElementClick);

            OnToolbarLeftInitialized();

            AddToolbarSpacer(true);

            OnBeforeToolbarRightInitialized();

            AddToolbarToggle("MiniMap", _miniMap.Visible, OnMiniMapValueChanged);
        }

        private void ReleaseToolbar()
        {
            if (_toolbar != null)
            {
                rootVisualElement.Remove(_toolbar);
            }

            _toolbar = null;
        }

        private void InitializeSearchProvider()
        {
            
            ReleaseSearchProvider();

            _searchProvider = CreateNodeSearchWindowProvider();
            
            _searchProvider.OnCreateNode = OnCreateNode;
        }

        private void ReleaseSearchProvider()
        {
            if (_searchProvider == null)
            {
                return;
            }

            DestroyImmediate(_searchProvider, true);
            _searchProvider = null;
        }

        #endregion

        #region Unity

        private void OnEnable()
        {
            
            InitializeGraphView();
            InitializeSearchProvider();

            OnWindowEnabled();

            NodeGraphEditorDataPostprocessor.NodeGraphEditorDataImported += OnNodeGraphEditorDataImported;
            NodeGraphEditorDataModificationProcessor.NodeGraphEditorDataWillDelete += OnNodeGraphEditorDataWillDelete;
            NodeGraphEditorDataModificationProcessor.NodeGraphEditorDataWillMove += OnNodeGraphEditorDataWillMove;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            rootVisualElement.RegisterCallback<MouseMoveEvent>(OnMouseMove);

            if (!IsLoaded)
            {
                return;
            }

            LoadGraph();
            InitializeToolbar();
            RestoreSelection();
            DelayedFrameGraphView();
        }

        private void OnDisable()
        {
            NodeGraphEditorDataPostprocessor.NodeGraphEditorDataImported -= OnNodeGraphEditorDataImported;
            NodeGraphEditorDataModificationProcessor.NodeGraphEditorDataWillDelete -= OnNodeGraphEditorDataWillDelete;
            NodeGraphEditorDataModificationProcessor.NodeGraphEditorDataWillMove -= OnNodeGraphEditorDataWillMove;
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;

            rootVisualElement.UnregisterCallback<MouseMoveEvent>(OnMouseMove);

            SaveGraphData();
            UnsubscribeGraph();
            ReleaseGraphView();
            ReleaseSearchProvider();
            ReleaseToolbar();
        }

        private void OnDestroy()
        {
            ResetSelection();
        }

        private void OnSelectionChange()
        {
            if (!IsLoaded || Selection.activeObject == GraphData)
            {
                return;
            }

            _graphView.ClearSelection(false);
            ResetSelection();
        }

        private void Update()
        {
            StartLastGraph();

            if (!IsLoaded)
            {
                return;
            }

            if (_graphDataIsDirty != EditorUtility.IsDirty(GraphData))
            {
                _graphDataIsDirty = !_graphDataIsDirty;
                _titleToolbarButton.text = $"{GraphData.name}{(_graphDataIsDirty ? "*" : string.Empty)}";
            }

            _graphView.Refresh();

            // Didn't find any suitable callback
            // So just waiting for a few updates to make sure the GraphView is fully drawn before trying to frame it
            if (_updateCountToFrameGraphView > 0 && --_updateCountToFrameGraphView <= 0)
            {
                _graphView.FrameAll();
            }
        }

        #endregion

        #region Copy/Paste

        private const float DEFAULT_PASTE_DISTANCE = 200f;

        private static string PasteDataKey => typeof(TNodeViewData).Name;

        private static string OnSerializeGraphElements(IEnumerable<GraphElement> elements)
        {
            var data = new NodeGraphPasteData<TNodeView, TNodeViewData, TNode>(elements);
            var json = EditorJsonUtility.ToJson(data);

            return $"{PasteDataKey} {json}";
        }

        private static bool OnCanPasteSerializedData(string json)
        {
            return json.StartsWith(PasteDataKey);
        }

        private void OnUnserializeAndPaste(string operationName, string json)
        {
            var data = new NodeGraphPasteData<TNodeView, TNodeViewData, TNode>();

            EditorJsonUtility.FromJsonOverwrite(json[PasteDataKey.Length..], data);

            if (data.Nodes.Count == 0)
            {
                Debug.LogWarning($"{GetType().Name}: Nothing to paste, node list is empty.");
            }

            var selectables = PoolUtility<List<ISelectable>>.Pull();

            using (DictionaryScope<string, string>.Create(out var oldToNewGuidMap))
            {
                var localMousePosition = _graphView.contentViewContainer.WorldToLocal(LastMousePosition);
                var rects = data.Nodes.Select(item => item.Position);
                var offset = operationName == "Paste" && TryEncompass(rects, out var rect)
                    ? localMousePosition - rect.center
                    : DEFAULT_PASTE_DISTANCE * Vector2.one;

                foreach (var node in data.Nodes)
                {
                    var oldGuid = node.Value.Guid;
                    var newGuid = Guid.NewGuid().ToString();

                    oldToNewGuidMap.Add(oldGuid, newGuid);
                    node.Value.Guid = newGuid;
                    node.Position = new Rect(node.Position.position + offset, node.Position.size);

                    var nodeView = CreateNodeView(node);

                    _graphView.AddNode(nodeView);
                    selectables.Add(nodeView);
                }

                foreach (var nodeEdge in data.Edges)
                {
                    if (!oldToNewGuidMap.TryGetValue(nodeEdge.InputNodeGuid, out var inputNodeGuid) ||
                        !oldToNewGuidMap.TryGetValue(nodeEdge.OutputNodeGuid, out var outputNodeGuid))
                    {
                        continue;
                    }

                    nodeEdge.InputNodeGuid = inputNodeGuid;
                    nodeEdge.OutputNodeGuid = outputNodeGuid;

                    if (_graphView.TryAddEdge(nodeEdge, out var edge))
                    {
                        selectables.Add(edge);
                    }
                }
            }

            if (selectables.Count > 0)
            {
                _graphView.ClearSelection(false);
                EditorApplication.delayCall += AddSelection;
            }
            else
            {
                PoolUtility<List<ISelectable>>.Push(selectables);
            }

            void AddSelection()
            {
                
                foreach (var selectable in selectables)
                {
                    _graphView.AddToSelection(selectable, false);
                }

                ChangeGraphSelection();

                selectables.Clear();
                PoolUtility<List<ISelectable>>.Push(selectables);
            }

            bool TryEncompass(IEnumerable<Rect> rects, out Rect rect)
            {
                var result = rects.Aggregate<Rect, Rect?>(default,
                    (current, rect) =>
                        current.HasValue
                            ? RectUtils.Encompass(current.Value, rect)
                            : rect);

                rect = result ?? default;

                return result.HasValue;
            }
        }

        #endregion

        #region Asset processing callbacks

        private void OnNodeGraphEditorDataImported(string path)
        {
            if (!IsLoaded || LastGraphPath != path)
            {
                return;
            }

            LoadGraph();
            RestoreSelection();
        }

        private void OnNodeGraphEditorDataWillDelete(string path)
        {
            if (LastGraphPath == path)
            {
                EditorPrefs.DeleteKey(LastGraphPathKey);
            }
        }

        private void OnNodeGraphEditorDataWillMove(string sourcePath, string destinationPath)
        {
            if (LastGraphPath == sourcePath)
            {
                LastGraphPath = destinationPath;
            }
        }

        private void OnUndoRedoPerformed()
        {
            if (!IsLoaded)
            {
                return;
            }

            LoadGraph();
            RestoreSelection();
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            LastMousePosition = evt.mousePosition;
        }

        #endregion

        #region Toolbar callbacks

        private void OnCreateNewButtonClick()
        {
            if (EditorUtility.DisplayDialog("Create new graph", "Are you sure? Current graph will be discard",
                    "Yes", "No"))
            {
                CreateGraph();
            }
        }

        private void OnLoadButtonClick()
        {
            var filePath = EditorUtility.OpenFilePanel("Load graph", DirectoryPath, "asset")
                ?.Replace(Application.dataPath, "Assets/");

            StartGraphAtPath(filePath);
        }

        private void OnSaveButtonClick()
        {
            SaveGraph(AssetDatabase.GetAssetPath(GraphData));
        }

        private void OnSaveAsButtonClick()
        {
            var fileName = IsDefault ? NewFileName : GraphData.name;
            var filePath = EditorUtility.SaveFilePanelInProject("Save graph", fileName, "asset",
                "Save graph", DirectoryPath);

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            SaveGraph(filePath);
            StartGraphAtPath(filePath);
        }

        private void OnTitleElementClick()
        {
            EditorGUIUtility.PingObject(GraphData);
        }

        private void OnMiniMapValueChanged(ChangeEvent<bool> changeEvent)
        {
            _miniMap.Visible = changeEvent.newValue;
        }

        #endregion
    }
}