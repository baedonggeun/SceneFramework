using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphRenderer
{
    NodeHierarchyBuilder hierarchyBuilder = new();
    NodeRenderer nodeRenderer = new();
    EdgeRenderer edgeRenderer = new();

    public Dictionary<PresetNodeData, List<INodeData>> ParentToChildrenMap { get; private set; }

    public enum ViewMode { None, Structure, Dependency }
    public ViewMode CurrentMode { get; set; } = ViewMode.None;

    private List<INodeData> nodes = new();

    public void BuildHierarchyNode(ScenePresetSO preset, Rect graphArea, GraphContext context)
    {
        if (preset == null) return;

        nodes = hierarchyBuilder.BuildFrom(preset, context);
        ParentToChildrenMap = hierarchyBuilder.ParentToChildrenMap;
    }

    public void BuildDependencyNode(List<DependencyNodeData> dependencyNodes)
    {
        nodes = dependencyNodes.Cast<INodeData>().ToList();
        ParentToChildrenMap = null;
    }

    public void DrawNode(GraphContext context)
    {
        nodeRenderer.DrawNode(nodes, context, CurrentMode);
    }

    public void DrawEdge(GraphContext context)
    {
        edgeRenderer.DrawEdges(nodes, context, CurrentMode);
    }

    public void Clear()
    {
        ParentToChildrenMap = null;
        nodes.Clear();
    }
}