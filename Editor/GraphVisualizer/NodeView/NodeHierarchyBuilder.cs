using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeHierarchyBuilder
{
    private readonly Dictionary<PresetNodeData, List<INodeData>> parentToChildrenMap = new();
    public Dictionary<PresetNodeData, List<INodeData>> ParentToChildrenMap => parentToChildrenMap;

    private readonly List<INodeData> flatNodes = new();

    private float nodeWidth = 300f;

    private float xSpacing;
    private float ySpacing;

    private float currentY = 0f;

      // 트리 구조 기반으로 노드 데이터 리스트 생성
    public List<INodeData> BuildFrom(ScenePresetSO root, GraphContext context)
    {
        flatNodes.Clear();
        parentToChildrenMap.Clear();
        currentY = 0f;

        xSpacing = context.XSpacing;
        ySpacing = context.YSpacing;

        if (root == null) return flatNodes;

        TraverseRecursive(root, 0);
        return flatNodes;
    }

      // 프리셋 및 하위 노드들을 재귀적으로 탐색하고 계층 위치 배치
    private float TraverseRecursive(ScenePresetSO preset, int depth)
    {
        var children = new List<INodeData>();

        float x = depth * (nodeWidth + xSpacing);
        List<float> childYs = new();

        // 서브 프리셋 처리 (중복 허용)
        foreach (var sub in preset.SubPresets)
        {
            float subY = TraverseRecursive(sub, depth + 1);
            var subNode = flatNodes.OfType<PresetNodeData>().Last(n => n.Preset == sub);
            children.Add(subNode);
            childYs.Add(subY);

            currentY = Mathf.Max(currentY, subNode.Position.y + subNode.Size.y + ySpacing);
        }

        // 플러그인 노드 배치 (리프 노드)
        foreach (var plugin in preset.Plugins)
        {
            var pluginNode = new PluginNodeData(plugin);
            float actualHeight = GraphLayoutUtil.CalculateTotalHeight(pluginNode, nodeWidth);

            pluginNode.Position = new Vector2((depth + 1) * (nodeWidth + xSpacing), currentY);
            pluginNode.Size = new Vector2(nodeWidth, actualHeight);

            flatNodes.Add(pluginNode);
            children.Add(pluginNode);
            childYs.Add(currentY);

            currentY += actualHeight + ySpacing;
        }



        // 프리셋 노드는 자식들의 중앙에 배치
        var presetNode = new PresetNodeData(preset);
        float presetHeight = GraphLayoutUtil.CalculateTotalHeight(presetNode, nodeWidth);

        float y;
        if (childYs.Count > 0)
        {
            y = childYs.Average();
        }
        else
        {
            y = currentY;
            currentY += presetHeight + ySpacing;
        }

        presetNode.Position = new Vector2(x, y);
        presetNode.Size = new Vector2(nodeWidth, presetHeight);

        flatNodes.Add(presetNode);

        // 부모-자식 관계 저장
        parentToChildrenMap[presetNode] = children;

        return y;
    }
}