using System.Collections.Generic;
using UnityEngine;

public class DependencyNode : NodeBase
{
    public override bool CanDraw(INodeData node) => node is DependencyNodeData;

    protected override List<NodeSection> BuildSections(INodeData node)
    {
        var depNode = node as DependencyNodeData;
        var sections = new List<NodeSection>();

        sections.Add(new NodeSection
        {
            Header = depNode.Type.Name,
            HeaderColor = Color.cyan,
            Type = NodeType.Dependency
        });

        // Inject된 타입 목록을 출력
        foreach (var target in depNode.InjectTargets)
        {
            sections.Add(new NodeSection
            {
                Header = $"- Injects → {target.Name}",
                HeaderColor = Color.red,
                Type = NodeType.Dependency,
                Items = new List<(string, GUIStyle, Color)>
                {
                    ($"{target.FullName}", NodeStyleUtil.GetNormalStyle(), Color.white)
                }
            });
        }

        return sections;
    }
}