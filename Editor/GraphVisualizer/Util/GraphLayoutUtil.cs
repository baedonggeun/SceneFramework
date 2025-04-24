using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//섹션 높이, 전체 노드 크기 계산
public static class GraphLayoutUtil
{
    public static float CalculateSectionHeight(NodeSection section, float width)
    {
        return section.Items.Sum(item => item.style.CalcHeight(new GUIContent(item.text), width)) + 20f;
    }

    public static float CalculateTotalHeight(List<NodeSection> sections, float width)
    {
        return sections.Sum(section => CalculateSectionHeight(section, width) + 4f);
    }

    public static float CalculateTotalHeight(INodeData node, float width)
    {
        float totalHeight = 0f;

        // 헤더: 타이틀 섹션
        totalHeight += 20f + 4f;

        var normalStyle = NodeStyleUtil.GetNormalStyle();

        foreach (var kv in node.ContentsByType)
        {
            float sectionHeight = 20f; // 헤더
            sectionHeight += kv.Value
                .Select(text => normalStyle.CalcHeight(new GUIContent($"- {text}"), width))
                .Sum();
            sectionHeight += 4f;
            totalHeight += sectionHeight;
        }

        return totalHeight;
    }
}