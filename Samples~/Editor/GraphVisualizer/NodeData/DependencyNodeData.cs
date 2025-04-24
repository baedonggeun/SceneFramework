using System;
using System.Collections.Generic;
using UnityEngine;

public class DependencyNodeData : INodeData
{
    public Type Type { get; private set; }
    public List<Type> InjectTargets { get; private set; }

    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }

    public string Label => Type.Name;

    public Dictionary<NodeType, string[]> ContentsByType
    {
        get
        {
            return new Dictionary<NodeType, string[]>
            {
                { NodeType.Service, InjectTargets.ConvertAll(t => t.Name).ToArray() }
            };
        }
    }

    public Color Color => new Color(1f, 1f, 0.4f);

    public UnityEngine.Object TargetObject
    {
        get
        {
            var scripts = UnityEditor.MonoImporter.GetAllRuntimeMonoScripts();
            foreach (var script in scripts)
            {
                if (script != null && script.GetClass() == Type)
                    return script;
            }
            return null;
        }
    }

    public DependencyNodeData ParentNode { get; set; }

    public DependencyNodeData(Type type, List<Type> injectTargets)
    {
        Type = type;
        InjectTargets = injectTargets;
    }
}