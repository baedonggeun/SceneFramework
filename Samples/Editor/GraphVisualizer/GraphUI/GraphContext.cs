using System.Collections.Generic;
using UnityEngine;

public class GraphContext
{
    public ScenePresetSO SelectedPreset { get; set; }

    public Dictionary<NodeType, bool> TypeFilterMap { get; set; } = new();

    public Rect GraphArea { get; set; } = new Rect(0, 0, 3000, 3000);

    public Dictionary<PresetNodeData, List<INodeData>> ParentToChildrenMap { get; set; }

    public INodeData SelectedNode { get; set; }
    public INodeData DraggingNode { get; set; }
    public Vector2 DragOffset { get; set; }
    public Vector2 ScrollOffset { get; set; } = Vector2.zero;
    public bool IsDraggingCanvas { get; set; } = false;
    public bool ShowEdges { get; set; } = false;

    public float XSpacing { get; set; } = 0f;
    public float YSpacing { get; set; } = 0f;

    public void Reset()
    {
        SelectedPreset = null;
        SelectedNode = null;
        ParentToChildrenMap = null;
        ShowEdges = false;
        GraphArea = Rect.zero;
        TypeFilterMap = null;
        XSpacing = 0;
        YSpacing = 0;
    }
}