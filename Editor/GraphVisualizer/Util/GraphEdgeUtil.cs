using UnityEditor;
using UnityEngine;

public static class GraphEdgeUtil
{
    private const float OffsetStep = 6f; // 노드 간 오프셋 간격

    // 구조 View에서 사용하는 베지어 곡선 연결선
    public static void DrawEdge(Vector2 from, Vector2 to, Color color, bool isHighlighted)
    {
        if (from.x > to.x)  //방향 보정
        {
            (from, to) = (to, from);
        }

        Vector2 dir = to - from;
        float distance = dir.magnitude * 0.3f;

        Vector2 tangentA = from + new Vector2(distance, 0f);
        Vector2 tangentB = to - new Vector2(distance, 0f);

        Handles.BeginGUI();
        Handles.DrawBezier(
            from, to,
            tangentA, tangentB,
            isHighlighted ? color : new Color(color.r, color.g, color.b, 0.2f),
            null,
            isHighlighted ? 4f : 4f
        );
        Handles.EndGUI();
    }

    // 노드 중심 좌표 반환 (스크롤 미포함)
    public static Vector2 GetNodeCenter(INodeData node, Vector2 scrollOffset)
    {
        return (node.Position - scrollOffset) + node.Size * 0.5f;
    }

    // 동일한 출발지 or 목적지를 가진 선들이 겹치지 않도록 좌표에 offset을 부여
    public static Vector2 ApplyOffset(Vector2 basePoint, int index, int groupCount, bool isVertical)
    {
        // 중앙 정렬 기반 오프셋
        float totalOffset = (groupCount - 1) * OffsetStep;
        float offset = index * OffsetStep - totalOffset / 2f;

        return isVertical
            ? basePoint + new Vector2(0f, offset)
            : basePoint + new Vector2(offset, 0f);
    }
}