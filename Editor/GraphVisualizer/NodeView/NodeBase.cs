using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class NodeBase : INode
{
    public abstract bool CanDraw(INodeData node);

    public virtual void Draw(INodeData node, GraphContext context)
    {
        Vector2 adjustedPos = node.Position - context.ScrollOffset;
        float width = node.Size.x;

        var sections = BuildSections(node);

        float height = GraphLayoutUtil.CalculateTotalHeight(sections, width);
        node.Size = new Vector2(width, height);
        Rect rect = new Rect(adjustedPos, node.Size);

        Color? highlightColor = GraphUtil.GetHighlightColor(node, context);

        DrawBackgroundBox(rect, highlightColor);
        DrawNodeSections(sections, adjustedPos, width, highlightColor);
        HandleClick(node, context, rect);
    }

    protected abstract List<NodeSection> BuildSections(INodeData node);

    protected virtual void DrawBackgroundBox(Rect rect, Color? highlightColor)
    {
        GUI.color = highlightColor ?? Color.white;
        GUI.Box(rect, GUIContent.none);
    }

    protected virtual void DrawNodeSections(List<NodeSection> sections, Vector2 startPos, float width, Color? highlightColor)
    {
        float yOffset = startPos.y;

        foreach (var section in sections)
        {
            float sectionHeight = GraphLayoutUtil.CalculateSectionHeight(section, width);
            Rect boxRect = new Rect(startPos.x + 2f, yOffset, width - 4f, sectionHeight);

            GUI.color = highlightColor ?? section.HeaderColor;
            GUI.Box(boxRect, GUIContent.none);

            GUI.color = highlightColor ?? section.HeaderColor;
            GUI.Label(
                new Rect(startPos.x + 8f, yOffset + 4f, width - 16f, 16f),
                section.Header,
                NodeStyleUtil.GetHeaderStyle()
            );

            float innerY = yOffset + 20f;
            foreach (var (text, style, color) in section.Items)
            {
                float height = style.CalcHeight(new GUIContent(text), width);
                GUI.color = color;
                GUI.Label(new Rect(startPos.x + 12f, innerY, width - 24f, height), text, style);
                innerY += height;
            }

            yOffset += sectionHeight + 4f;
        }

        GUI.color = Color.white;
    }

    protected virtual void HandleClick(INodeData node, GraphContext context, Rect rect)
    {
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            context.SelectedNode = node;
            EditorGUIUtility.PingObject(node.TargetObject);
            Selection.activeObject = node.TargetObject;
            Event.current.Use();
        }
    }
}
public class NodeSection
{
    public string Header;
    public Color HeaderColor;
    public NodeType Type;
    public List<(string text, GUIStyle style, Color color)> Items = new();
}