using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class InjectRunner
{
    private static readonly HashSet<object> injectedObjects = new();

    public static void TryInject(object target, Dictionary<Type, object> registry)
    {
        if (target == null || registry == null)
            return;

        Type targetType = target.GetType();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        foreach (FieldInfo field in targetType.GetFields(flags))
        {
            // 이미 값이 존재하면 주입 생략
            if (field.GetValue(target) != null)
                continue;

            Type dependencyType = field.FieldType;

            if (registry.TryGetValue(dependencyType, out object resolvedInstance))
                field.SetValue(target, resolvedInstance);
        }
    }

    public static void TryInjectAll(object target, Dictionary<Type, object> registry)
    {
        if (target == null || registry == null)
            return;

        if (injectedObjects.Contains(target))
            return;

        injectedObjects.Add(target);
        TryInject(target, registry);
    }

    // GameObject 및 모든 하위 MonoBehaviour에 대해 주입 실행
    public static void TryInjectAll(GameObject gameObject, Dictionary<Type, object> registry)
    {
        if (gameObject == null || registry == null)
            return;

        var components = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var comp in components)
        {
            if (comp == null || injectedObjects.Contains(comp)) continue;
            injectedObjects.Add(comp);
            TryInject(comp, registry);
        }
    }

    public static void ClearCache() => injectedObjects.Clear();
}