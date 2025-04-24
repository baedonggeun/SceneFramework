using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PresetNode : NodeBase
{
    public override bool CanDraw(INodeData node) => node is PresetNodeData;

    protected override List<NodeSection> BuildSections(INodeData node)
    {
        var presetNode = node as PresetNodeData;
        var sections = new List<NodeSection>();

        sections.Add(new NodeSection
        {
            Header = presetNode.Label,
            HeaderColor = new Color(0.53f, 0.81f, 0.92f),
            Type = NodeType.Preset
        });

        foreach (var kv in presetNode.ContentsByType)
        {
            var section = new NodeSection
            {
                Header = $"[{kv.Key}]",
                HeaderColor = NodeColorUtil.GetColorForNodeType(kv.Key),
                Type = kv.Key
            };

            section.Items.AddRange(kv.Value.Select(text => (
                $"- {text}",
                NodeStyleUtil.GetNormalStyle(),
                Color.white
            )));

            sections.Add(section);
        }

        return sections;
    }
}