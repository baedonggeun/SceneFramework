using UnityEngine;
using System.Collections.Generic;

//GUIStyle 캐싱, 배경 박스 스타일
public static class NodeStyleUtil
{
    private static readonly Dictionary<NodeType, GUIStyle> _filterButtonStyleCache = new();
    private static GUIStyle _normalStyle;
    private static GUIStyle _headerStyle;

    public static GUIStyle GetNormalStyle()
    {
        if (_normalStyle == null)
        {
            _normalStyle = new GUIStyle(GUI.skin.label) { fontSize = 11 };
        }
        return _normalStyle;
    }

    public static GUIStyle GetHeaderStyle()
    {
        if (_headerStyle == null)
        {
            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
        }
        return _headerStyle;
    }

    public static GUIStyle GetFilterButtonStyle(NodeType type)
    {
        if (!_filterButtonStyleCache.TryGetValue(type, out var style))
        {
            var color = NodeColorUtil.GetColorForNodeType(type);
            style = new GUIStyle("Button")
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = color },
                hover = { textColor = color },
                active = { textColor = color }
            };
            _filterButtonStyleCache[type] = style;
        }
        return style;
    }
}