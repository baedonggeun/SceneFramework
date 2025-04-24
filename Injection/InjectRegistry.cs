using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class InjectRegistry
{
    private readonly Dictionary<Type, object> map = new();

    // 인스턴스 등록
    public void Register(Type type, object instance)
    {
        if (type == null || instance == null) return;
        map[type] = instance;
    }

    public void Register<T>(T instance) => Register(typeof(T), instance);

    // 인스턴스 조회
    public bool TryGet<T>(out T instance)
    {
        if (map.TryGetValue(typeof(T), out var obj) && obj is T casted)
        {
            instance = casted;
            return true;
        }

        instance = default;
        return false;
    }

    public object Get(Type type) => map.TryGetValue(type, out var obj) ? obj : null;

    public Dictionary<Type, object> AsDictionary() => map;

    // Preset 기반 자동 구성
    public static InjectRegistry BuildFromScenePreset(ScenePresetSO scenePreset)
    {
        var registry = new InjectRegistry();

        if (scenePreset == null)
        {
            Debug.LogError("[InjectRegistry] ScenePresetSO is null");
            return registry;
        }

        foreach (var plugin in scenePreset.GetAllPlugins())
        {
            if (plugin is not ServicePluginSO servicePlugin) continue;

            foreach (var info in servicePlugin.RequiredServices)
            {
                Type type = Type.GetType(info.typeName);
                if (type == null)
                {
                    Debug.LogWarning($"[InjectRegistry] Type not found: {info.typeName}");
                    continue;
                }

                // 씬 한정 서비스면 기존 인스턴스 제거
                if (info.isSceneScoped)
                {
                    Type generic = typeof(MonoSingleton<>).MakeGenericType(type);
                    MethodInfo reset = generic.GetMethod("ResetInstance", BindingFlags.Public | BindingFlags.Static);
                    reset?.Invoke(null, null);
                }

                // 인스턴스 생성
                Type createGeneric = typeof(MonoSingleton<>).MakeGenericType(type);
                createGeneric.GetMethod("ForceCreate", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);

                object instance = createGeneric.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
                if (instance == null)
                {
                    Debug.LogWarning($"[InjectRegistry] Instance is null: {type.Name}");
                    continue;
                }

                // 구체 타입 등록
                registry.Register(type, instance);

                // 인터페이스 타입 등록
                foreach (var iface in type.GetInterfaces())
                {
                    if (iface == typeof(IInjectable) || iface == typeof(IInitializable)) continue;

                    if (!registry.map.ContainsKey(iface))
                    {
                        registry.Register(iface, instance);
                    }
                }
            }
        }

        return registry;
    }
}