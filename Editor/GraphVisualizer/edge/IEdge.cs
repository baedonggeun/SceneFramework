using System.Collections.Generic;

public interface IEdge
{
    GraphRenderer.ViewMode Mode { get; }
    void DrawEdges(List<INodeData> nodes, GraphContext context);
}