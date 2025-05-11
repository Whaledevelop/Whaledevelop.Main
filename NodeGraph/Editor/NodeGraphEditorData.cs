using System.Collections.Generic;
using System.Linq;
using Whaledevelop.Extensions;
using Whaledevelop.Scopes;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Whaledevelop.NodeGraph
{
    public abstract class NodeGraphEditorData : ScriptableObject
    {
        public abstract INodeGraphSelection NodeGraphSelection { get; }

        [Button]
        [PropertyOrder(-10)]
        public abstract void OpenInGraph();

        [Button]
        [PropertyOrder(-9)]
        protected abstract void OpenGeneratedFile();

        [Button]
        [PropertyOrder(-8)]
        public abstract void ApplyToSettings();
    }

    public abstract class NodeGraphEditorData<TNodeViewData, TNode> : NodeGraphEditorData
        where TNodeViewData : NodeViewData<TNode>, new()
        where TNode : BaseNode
    {
        [HideInInspector]
        [SerializeReference]
        private List<TNodeViewData> _nodes = new();

        [HideInInspector]
        [SerializeField]
        private List<NodeEdge> _edges = new();

        // ReSharper disable once ConvertToAutoProperty
        public List<TNodeViewData> Nodes => _nodes;

        public List<NodeEdge> Edges => _edges;

        protected virtual void OnTransformToGraphData(IList<TNode> nodes, IList<NodeEdge> edges)
        {
        }

        protected void TransformToGraphData<TGraphDataSettings, TNodeGraphData>(GraphDataSettings<TGraphDataSettings, TNodeGraphData, TNode> graphDataSettings)
            where TGraphDataSettings : class, IGraphDataSettings<TGraphDataSettings, TNodeGraphData, TNode>
            where TNodeGraphData : NodeGraphData<TNode>, new()
        {
            using (ListScope<TNode>.Create(out var nodes))
            using (ListScope<NodeEdge>.Create(out var edges))
            {
                Nodes.Select(data => data.Value).ToList(nodes);
                Edges.ToList(edges);

                OnTransformToGraphData(nodes, edges);

                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);

                using (DictionaryScope<object, long>.Create(out var referenceToIdMap))
                using (DictionaryScope<string, long>.Create(out var nodeGuidToIdMap))
                {
                    var editorReferenceIds = ManagedReferenceUtility.GetManagedReferenceIds(this);
                    var settingsReferenceIds = ManagedReferenceUtility.GetManagedReferenceIds(graphDataSettings);

                    FillEditorReferences(this, editorReferenceIds, referenceToIdMap);
                    FillSettingsKeys(graphDataSettings, settingsReferenceIds, nodeGuidToIdMap);

                    var graph = graphDataSettings.Graph;

                    graph.Reset();

                    EditorUtility.SetDirty(graphDataSettings);
                    AssetDatabase.SaveAssetIfDirty(graphDataSettings);

                    graph.Set(nodes, edges);

                    foreach (var node in graph.Nodes)
                    {
                        if (nodeGuidToIdMap.TryGetValue(node.Guid, out var id))
                        {
                            ManagedReferenceUtility.SetManagedReferenceIdForObject(graphDataSettings, node, id);
                        }
                    }

                    foreach (var (reference, id) in referenceToIdMap)
                    {
                        if (ManagedReferenceUtility.GetManagedReferenceIdForObject(graphDataSettings, reference) == ManagedReferenceUtility.RefIdUnknown)
                        {
                            ManagedReferenceUtility.SetManagedReferenceIdForObject(graphDataSettings, reference, id);
                        }
                    }

                    EditorUtility.SetDirty(graphDataSettings);
                    AssetDatabase.SaveAssetIfDirty(graphDataSettings);
                }
            }
        }

        private static void FillEditorReferences(Object host, IEnumerable<long> referenceIds, IDictionary<object, long> referenceToIdMap)
        {
            foreach (var id in referenceIds)
            {
                if (id == ManagedReferenceUtility.RefIdNull)
                {
                    continue;
                }

                var reference = ManagedReferenceUtility.GetManagedReference(host, id);

                if (reference != null)
                {
                    referenceToIdMap.Add(reference, id);
                }
            }

        }
        private static void FillSettingsKeys(Object host, IEnumerable<long> referenceIds, IDictionary<string, long> nodeGuidToIdMap)
        {
            foreach (var id in referenceIds)
            {
                if (id == ManagedReferenceUtility.RefIdNull)
                {
                    continue;
                }

                var reference = ManagedReferenceUtility.GetManagedReference(host, id);

                if (reference is BaseNode node)
                {
                    nodeGuidToIdMap.Add(node.Guid, id);
                }
            }
        }
    }
}