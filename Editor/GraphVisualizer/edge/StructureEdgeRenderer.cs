using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class StructureEdgeRenderer : IEdge
{
    public GraphRenderer.ViewMode Mode => GraphRenderer.ViewMode.Structure;

    public void DrawEdges(List<INodeData> nodes, GraphContext context)
    {
        if (context.ParentToChildrenMap == null) return;

        foreach (var kvp in context.ParentToChildrenMap)
        {
            var parentNode = kvp.Key;
            var children = kvp.Value;
            var parentCenter = GraphEdgeUtil.GetNodeCenter(parentNode, context.ScrollOffset);

            foreach (var childNode in children)
            {
                var childCenter = GraphEdgeUtil.GetNodeCenter(childNode, context.ScrollOffset);

                bool isHighlighted = GraphUtil.IsConnectedEdge(parentNode, childNode, context.SelectedNode);
                Color lineColor = (childNode is PluginNodeData) ? new Color(1.0f, 0.55f, 0.31f) : new Color(0.53f, 0.81f, 0.92f);

                GraphEdgeUtil.DrawEdge(parentCenter, childCenter, lineColor, isHighlighted);
            }
        }
    }
}