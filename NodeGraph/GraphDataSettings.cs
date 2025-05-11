using Sirenix.OdinInspector;
using UnityEngine;

namespace Whaledevelop.NodeGraph
{
    public abstract class GraphDataSettings<TGraphDataSettings, TNodeGraphData, TNode> :
        ScriptableObject, IGraphDataSettings<TGraphDataSettings, TNodeGraphData, TNode>
        where TGraphDataSettings : class, IGraphDataSettings<TGraphDataSettings, TNodeGraphData, TNode>
        where TNodeGraphData : NodeGraphData<TNode>, new()
        where TNode : BaseNode
    {
        [SerializeField]
        [Required]
        [ReadOnly]
        private TNodeGraphData _graph = new();

        public TNodeGraphData Graph => _graph;

        #region IGraphDataSettings<TGraphDataSettings, TNodeGraphData, TNode>

        TNodeGraphData IGraphDataSettings<TGraphDataSettings, TNodeGraphData, TNode>.Graph => _graph;

        #endregion
    }
}