using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DependencyEdgeRenderer : IEdge
{
    public GraphRenderer.ViewMode Mode => GraphRenderer.ViewMode.Dependency;

    public void DrawEdges(List<INodeData> nodes, GraphContext context)
    {
        var dependencyNodes = nodes.OfType<DependencyNodeData>().ToList();
        if (dependencyNodes.Count == 0) return;

        var edges = new List<(DependencyNodeData consumer, DependencyNodeData provider)>();

        foreach (var node in dependencyNodes)
        {
            var parent = node.ParentNode;
            if (parent == null) continue;

            var from = GraphEdgeUtil.GetNodeCenter(parent, context.ScrollOffset);
            var to = GraphEdgeUtil.GetNodeCenter(node, context.ScrollOffset);

            var fromOffset = GraphEdgeUtil.ApplyOffset(from, 0, 1, false);
            var toOffset = GraphEdgeUtil.ApplyOffset(to, 0, 1, false);

            bool isHighlighted = GraphUtil.IsDependencyConnectedEdge(parent, node, context.SelectedNode);
            GraphEdgeUtil.DrawEdge(fromOffset, toOffset, Color.yellow, isHighlighted);
        }

        var groupedByProvider = edges
            .GroupBy(e => e.provider)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var provider in groupedByProvider.Keys)
        {
            var group = groupedByProvider[provider];

            for (int i = 0; i < group.Count; i++)
            {
                var consumer = group[i].consumer;

                var fromCenter = GraphEdgeUtil.GetNodeCenter(consumer, context.ScrollOffset);
                var toCenter = GraphEdgeUtil.GetNodeCenter(provider, context.ScrollOffset);

                var fromOffset = GraphEdgeUtil.ApplyOffset(fromCenter, i, group.Count, isVertical: false);
                var toOffset = GraphEdgeUtil.ApplyOffset(toCenter, i, group.Count, isVertical: false);

                bool isHighlighted = GraphUtil.IsDependencyConnectedEdge(consumer, provider, context.SelectedNode);
                Color lineColor = Color.yellow;

                GraphEdgeUtil.DrawEdge(fromOffset, toOffset, lineColor, isHighlighted);
            }
        }
    }
}