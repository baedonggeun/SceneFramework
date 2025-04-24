using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(menuName = "HybridSceneFramework/Plugins/Service")]
public class ServicePluginSO : ScenePluginSO
{
    [Header("필요한 매니저 타입 이름 (MonoSingleton)")]
    [SerializeField] private ServiceInfo[] requiredServices;
    public ServiceInfo[] RequiredServices => requiredServices;

    [Serializable]
    public class ServiceInfo
    {
        [Header("MonoSingleton으로 등록된 서비스 타입 이름")]
        public string typeName;

        [Header("씬 한정 서비스인지 여부 (true면 씬 전환 시 자동 언로드)")]
        public bool isSceneScoped = true;
    }

    private readonly List<Type> activeSceneScopedTypes = new();

    // 씬 진입 시 자동으로 서비스들을 생성하고 초기화
    public override async UniTask OnLoad(ScenePresetSO context, Dictionary<Type, object> instanceMap)
    {
        if (instanceMap == null)
        {
            Debug.LogError("[ServicePluginSO] instanceMap이 null입니다.");
            return;
        }

        // 1. 해당 플러그인에서 정의한 타입만 대상으로 제한
        var pluginServiceTypes = requiredServices
            .Select(info => Type.GetType(info.typeName))
            .Where(t => t != null)
            .ToHashSet();

        // 씬 스코프 서비스 타입을 저장
        activeSceneScopedTypes.Clear();
        foreach (var info in requiredServices)
        {
            Type type = Type.GetType(info.typeName);
            if (type != null && info.isSceneScoped)
                activeSceneScopedTypes.Add(type);
        }

        //2. 필터링된 서비스들을 가져와서 위상 정렬
        var autoInitializables = instanceMap
            .Where(kv => pluginServiceTypes.Contains(kv.Key))
            .Select(kv => kv.Value)
            .OfType<IInitializable>()
            .Where(init => init.AutoInitialize)
            .OfType<IInjectable>()
            .ToList();

        // 3. 위상 정렬 적용
        var sorted = InjectSortingUtil.TopologicalSort(autoInitializables);

        // 4. 정렬된 순서대로 주입 + 초기화
        foreach (var injectable in sorted)
        {
            // 필드 주입
            InjectRunner.TryInjectAll(injectable, instanceMap);

            // 초기화
            if (injectable is IInitializable init)
            {
                try
                {
                    await init.InitializeAsync();
                    Debug.Log($"[ServicePluginSO] Initialized: {injectable.GetType().Name}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ServicePluginSO] Init 실패: {injectable.GetType().Name} - {ex.Message}");
                }
            }
        }
    }

    // 씬 언로드 시 서비스들을 정리
    public override UniTask OnUnload(ScenePresetSO context)
    {
        foreach (Type type in activeSceneScopedTypes)
        {
            Type generic = typeof(MonoSingleton<>).MakeGenericType(type);
            MethodInfo reset = generic.GetMethod("ResetInstance", BindingFlags.Public | BindingFlags.Static);
            reset?.Invoke(null, null);
        }

        activeSceneScopedTypes.Clear();
        return UniTask.CompletedTask;
    }
}