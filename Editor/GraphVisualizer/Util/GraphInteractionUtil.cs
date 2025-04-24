using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class GraphInteractionUtil
{
    public static void HandleMouseEvents(List<INodeData> nodes, GraphContext context)
    {
        Event e = Event.current;
        if (e == null) return;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            bool clickedOnNode = false;
            foreach (var node in nodes)
            {
                Rect rect = new Rect(node.Position - context.ScrollOffset, node.Size);
                if (rect.Contains(e.mousePosition))
                {
                    context.DraggingNode = node;
                    context.DragOffset = e.mousePosition - node.Position + context.ScrollOffset;
                    context.IsDraggingCanvas = false;
                    clickedOnNode = true;
                    e.Use();
                    break;
                }
            }

            if (!clickedOnNode)
            {
                context.IsDraggingCanvas = true;
                context.DragOffset = e.mousePosition;
                e.Use();
            }
        }

        if (e.type == EventType.MouseDrag && e.button == 0)
        {
            if (context.IsDraggingCanvas)
            {
                Vector2 delta = e.mousePosition - context.DragOffset;
                context.ScrollOffset -= delta;
                context.DragOffset = e.mousePosition;
                e.Use();
            }
            else if (context.DraggingNode != null)
            {
                context.DraggingNode.Position = e.mousePosition - context.DragOffset + context.ScrollOffset;
                e.Use();
            }
        }

        if (e.type == EventType.MouseUp && e.button == 0)
        {
            context.DraggingNode = null;
            context.IsDraggingCanvas = false;
            e.Use();
        }

        if (e.type == EventType.MouseDown && e.button == 1)
        {
            bool clickedOnNode = false;
            foreach (var node in nodes)
            {
                Rect rect = new Rect(node.Position - context.ScrollOffset, node.Size);
                if (rect.Contains(e.mousePosition))
                {
                    context.SelectedNode = node;
                    EditorGUIUtility.PingObject(node.TargetObject);
                    Selection.activeObject = node.TargetObject;
                    clickedOnNode = true;
                    e.Use();
                    break;
                }
            }

            if (!clickedOnNode)
            {
                context.SelectedNode = null;
                e.Use();
            }
        }
    }
}