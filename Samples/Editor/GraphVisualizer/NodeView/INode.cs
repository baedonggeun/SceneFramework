using UnityEngine;

public interface INode
{
    bool CanDraw(INodeData node);
    void Draw(INodeData node, GraphContext context);
}