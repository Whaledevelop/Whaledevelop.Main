namespace Whaledevelop.NodeGraph
{
    public interface IGraphDataSettings<TGraphDataSettings, out TNodeGraphData, TNode>
        where TGraphDataSettings : class, IGraphDataSettings<TGraphDataSettings, TNodeGraphData, TNode>
        where TNodeGraphData : NodeGraphData<TNode>
        where TNode : BaseNode
    {
        TNodeGraphData Graph { get; }
    }
}