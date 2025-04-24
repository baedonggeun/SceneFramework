using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PluginNode : NodeBase
{
    public override bool CanDraw(INodeData node) => node is PluginNodeData;

    protected override List<NodeSection> BuildSections(INodeData node)
    {
        var pluginNode = node as PluginNodeData;
        var sections = new List<NodeSection>();

        sections.Add(new NodeSection
        {
            Header = pluginNode.Label,
            HeaderColor = new Color(1.0f, 0.55f, 0.31f),
            Type = NodeType.Plugin
        });

        foreach (var kv in pluginNode.ContentsByType)
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