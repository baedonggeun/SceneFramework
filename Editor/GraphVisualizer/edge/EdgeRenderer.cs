using System.Collections.Generic;
using UnityEngine;

public class EdgeRenderer
{
    private readonly Dictionary<GraphRenderer.ViewMode, IEdge> edges = new()
    {
        { GraphRenderer.ViewMode.Structure, new StructureEdgeRenderer() },
        { GraphRenderer.ViewMode.Dependency, new DependencyEdgeRenderer() }
    };

    public void DrawEdges(List<INodeData> nodes, GraphContext context, GraphRenderer.ViewMode mode)
    {
        if (!context.ShowEdges || mode == GraphRenderer.ViewMode.None)
            return;

        if (edges.TryGetValue(mode, out var drawer))
        {
            drawer.DrawEdges(nodes, context);
        }
    }
}