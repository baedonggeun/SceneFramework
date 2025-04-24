using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GraphWindow : EditorWindow
{
    private GraphRenderer renderer = new();
    private GraphContext context = new();

    private Dictionary<NodeType, bool> typeFilterMap = new()
    {
        { NodeType.Preset,         false },
        { NodeType.Service,           false },
        { NodeType.AddressableSO,     false },
        { NodeType.AddressableUI,     false },
        { NodeType.AddressablePrefab, false }
    };

    private const float margin = 20f;

    [MenuItem("Tools/Hybrid Scene Framework/Graph Viewer")]
    public static void ShowWindow()
    {
        var window = GetWindow<GraphWindow>();
        window.titleContent = new GUIContent("Graph Viewer");
        window.Show();
    }

    private void OnGUI()
    {
        context.TypeFilterMap = typeFilterMap;

        float fullWidth = position.width;
        float leftPanelWidth = 280f;
        float rightPanelWidth = fullWidth - leftPanelWidth - margin;

        GUILayout.BeginHorizontal();

        // LEFT PANEL
        GUILayout.BeginVertical(GUILayout.Width(leftPanelWidth));
        GUILayout.Space(10);

        DrawModeSelector(leftPanelWidth - margin); // [Structure] / [Dependency]

        switch (renderer.CurrentMode)
        {
            case GraphRenderer.ViewMode.Structure:
                DrawStructurePanel(leftPanelWidth - margin);
                break;
            case GraphRenderer.ViewMode.Dependency:
                DrawDependencyPanel(leftPanelWidth - margin);
                break;
        }

        GUILayout.EndVertical();

        // RIGHT PANEL - Graph Drawing
        Rect area = new Rect(leftPanelWidth + margin, 10f, rightPanelWidth, position.height - 20f);
        context.GraphArea = area;

        GUILayout.BeginVertical(GUILayout.Width(rightPanelWidth));
        GUI.BeginGroup(area);
        renderer.DrawNode(context);
        renderer.DrawEdge(context);
        GUI.EndGroup();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawModeSelector(float width)
    {
        GUILayout.Label("[Select Graph Mode]", EditorStyles.boldLabel);

        if (GUILayout.Toggle(renderer.CurrentMode == GraphRenderer.ViewMode.Structure, "Preset Structure", "Button", GUILayout.Width(width)))
        {
            if (renderer.CurrentMode != GraphRenderer.ViewMode.Structure)
            {
                ClearGraph();
                renderer.CurrentMode = GraphRenderer.ViewMode.Structure;
            }
        }

        if (GUILayout.Toggle(renderer.CurrentMode == GraphRenderer.ViewMode.Dependency, "Component Dependency", "Button", GUILayout.Width(width)))
        {
            if (renderer.CurrentMode != GraphRenderer.ViewMode.Dependency)
            {
                ClearGraph();
                renderer.CurrentMode = GraphRenderer.ViewMode.Dependency;
            }
        }

        GUILayout.Space(10);
    }

    private void ClearGraph()
    {
        context.Reset();
        renderer.Clear();
    }

    private void DrawStructurePanel(float width)
    {
        DrawPresetSelector(width);
        DrawSpacingInputs(width);
        DrawColorFilterToggles(width);
    }

    private void DrawDependencyPanel(float width)
    {
        GUILayout.Space(10);
        GUILayout.Label("[Dependency Inject Graph]", EditorStyles.boldLabel);

        if (GUILayout.Button("Build Inject Graph", GUILayout.Width(width)))
        {
            var dependencyNodes = DependencyGraphBuilder.Build(context);
            context.SelectedNode = null;
            context.ShowEdges = true;

            renderer.BuildDependencyNode(dependencyNodes);
            renderer.CurrentMode = GraphRenderer.ViewMode.Dependency;
        }
    }

    private void DrawPresetSelector(float width)
    {
        GUILayout.Label("[Select ScenePresetSO]", EditorStyles.boldLabel);
        context.SelectedPreset = (ScenePresetSO)EditorGUILayout.ObjectField("Preset", context.SelectedPreset, typeof(ScenePresetSO), false, GUILayout.Width(width));

        if (GUILayout.Button("Create Graph", GUILayout.Width(width)) && context.SelectedPreset != null)
        {
            renderer.BuildHierarchyNode(context.SelectedPreset, GetAvailableGraphArea(), context);
            context.ParentToChildrenMap = renderer.ParentToChildrenMap;
            context.SelectedNode = null;
            context.ShowEdges = true;
            renderer.CurrentMode = GraphRenderer.ViewMode.Structure;
        }
    }

    private void DrawSpacingInputs(float width)
    {
        GUILayout.BeginVertical();
        GUILayout.Label("[Node Interval Setting]", EditorStyles.boldLabel);

        context.XSpacing = DrawSpacingField("X Spacing", context.XSpacing, width);
        context.YSpacing = DrawSpacingField("Y Spacing", context.YSpacing, width);

        GUILayout.EndVertical();
    }

    private float DrawSpacingField(string label, float value, float width)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(70));
        value = EditorGUILayout.FloatField(value, GUILayout.Width(width - 80));
        GUILayout.EndHorizontal();
        return value;
    }

    private void DrawColorFilterToggles(float width)
    {
        GUILayout.Label("[Node Highlight]", EditorStyles.boldLabel);

        foreach (var type in typeFilterMap.Keys.ToList())
        {
            string label = $"{type}";
            var style = NodeStyleUtil.GetFilterButtonStyle(type); // 유틸 기반 캐싱
            typeFilterMap[type] = GUILayout.Toggle(typeFilterMap[type], label, style, GUILayout.Width(width));
        }
    }

    private Rect GetAvailableGraphArea()
    {
        float uiHeight = 200f;
        float padding = 10f;
        return new Rect(
            x: padding,
            y: uiHeight,
            width: position.width - padding * 2,
            height: position.height - uiHeight - padding
        );
    }
}