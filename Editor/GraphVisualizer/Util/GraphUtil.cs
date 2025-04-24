using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//구조 탐색, 필터링 유틸
public static class GraphUtil
{
    // 현재 트리에 존재하는 모든 PluginNode 중 필터 조건을 만족하는 노드만 반환
    public static bool IsNodeFilteredByPlugin(
        INodeData node,
        System.Func<ScenePluginSO, bool> condition,
        Dictionary<PresetNodeData, List<INodeData>> map)
    {
        if (condition == null || node == null) return false;

        if (node is PluginNodeData pluginNode)
            return condition(pluginNode.Plugin);

        if (node is PresetNodeData preset && map.TryGetValue(preset, out var children))
            return children.Any(child => IsNodeFilteredByPlugin(child, condition, map));

        return false;
    }

    public static bool IsNodeWithinOneDepth(INodeData current, INodeData selected, Dictionary<PresetNodeData, List<INodeData>> map)
    {
        if (current == null || selected == null)
            return false;

        if (current == selected)
            return true;

        // 자식 확인 (selected가 부모)
        if (selected is PresetNodeData selectedPreset &&
            map.TryGetValue(selectedPreset, out var children) &&
            children.Contains(current))
            return true;

        // 부모 확인 (selected가 자식)
        if (current is PresetNodeData currentPreset &&
            map.TryGetValue(currentPreset, out var childList) &&
            childList.Contains(selected))
            return true;

        return false;
    }

    public static bool IsConnectedEdge(PresetNodeData parent, INodeData child, INodeData selected)
    {
        return selected != null && (selected == parent || selected == child);
    }

    public static bool IsDependencyConnectedEdge(DependencyNodeData from, DependencyNodeData to, INodeData selected)
    {
        return selected != null && (selected == from || selected == to);
    }

    public static Color? GetHighlightColor(INodeData node, GraphContext context)
    {
        if (IsNodeWithinOneDepth(node, context.SelectedNode, context.ParentToChildrenMap))
            return Color.yellow;

        if (node is PluginNodeData pluginNode &&
            context.TypeFilterMap.TryGetValue(DetermineNodeType(pluginNode.Plugin), out bool pluginFiltered) && pluginFiltered)
            return Color.yellow;

        if (node is PresetNodeData &&
            context.TypeFilterMap.TryGetValue(NodeType.Preset, out bool presetFiltered) && presetFiltered)
            return Color.yellow;

        return null;
    }

    public static NodeType DetermineNodeType(ScenePluginSO plugin)
    {
        return plugin switch
        {
            ServicePluginSO => NodeType.Service,
            AddressableSOPluginSO => NodeType.AddressableSO,
            AddressableUIPluginSO => NodeType.AddressableUI,
            AddressablePrefabPluginSO => NodeType.AddressablePrefab,
            _ => NodeType.Unknown
        };
    }

    public static HashSet<DependencyNodeData> GetDependencyDepth1Connected(DependencyNodeData selectedNode, List<DependencyNodeData> allNodes)
    {
        var result = new HashSet<DependencyNodeData>();
        if (selectedNode == null) return result;

        result.Add(selectedNode);

        // depth - 1: 부모
        if (selectedNode.ParentNode != null)
            result.Add(selectedNode.ParentNode);

        // depth + 1: 자식
        var children = allNodes.Where(n => n.ParentNode == selectedNode);
        foreach (var child in children)
            result.Add(child);

        return result;
    }
}