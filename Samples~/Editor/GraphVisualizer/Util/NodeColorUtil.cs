using System.Collections.Generic;
using UnityEngine;

//타입별 논리적 색상 정의 (데이터 중심)
public static class NodeColorUtil
{
    private static readonly Dictionary<NodeType, Color> nodeTypeColorMap = new()
    {
        { NodeType.Preset,             new Color(0.53f, 0.81f, 0.92f) },
        { NodeType.Plugin,             new Color(1.0f, 0.55f, 0.31f) },
        { NodeType.Service,            new Color(0.5f, 1f, 0.7f) },
        { NodeType.AddressableSO,      new Color(1.0f, 0.32f, 0.5f) },
        { NodeType.AddressableUI,      new Color(0.7f, 0.5f, 1f) },
        { NodeType.AddressablePrefab,  new Color(0.5f, 0.7f, 1f) },
        { NodeType.Unknown,            Color.gray }
    };

    public static Color GetColorForNodeType(NodeType type)
    {
        return nodeTypeColorMap.TryGetValue(type, out var color) ? color : Color.white;
    }

    public static Color GetColorForPreset(SceneKey sceneType)
    {
        if (sceneType == SceneKey.Global)
            return GetColorForNodeType(NodeType.Preset);

        return GetColorForNodeType(NodeType.Unknown);
    }

    public static Color GetColorForPlugin(NodeType type)
    {
        return GetColorForNodeType(type switch
        {
            NodeType.Service => NodeType.Service,
            NodeType.AddressableSO => NodeType.AddressableSO,
            NodeType.AddressableUI => NodeType.AddressableUI,
            NodeType.AddressablePrefab => NodeType.AddressablePrefab,
            _ => NodeType.Unknown
        });
    }
}