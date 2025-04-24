using System.Collections.Generic;
using UnityEngine;

public class NodeRenderer
{
    private readonly List<INode> nodeDrawers = new()
    {
        new PresetNode(),
        new PluginNode(),
        new DependencyNode()
    };

    public void DrawNode(List<INodeData> nodes, GraphContext context, GraphRenderer.ViewMode viewMode)
    {
        GraphInteractionUtil.HandleMouseEvents(nodes, context);

        foreach (var node in nodes)
        {
            foreach (var drawer in nodeDrawers)
            {
                if (drawer.CanDraw(node))
                {
                    drawer.Draw(node, context);
                    break;
                }
            }
        }
    }
}