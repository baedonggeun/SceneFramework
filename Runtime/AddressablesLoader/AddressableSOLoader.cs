using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Cysharp.Threading.Tasks;

public static class AddressableSOLoader
{
    // 개별 SO 키 기반 로딩 캐시 (preloadInfos 전용)
    private static readonly Dictionary<Type, Dictionary<string, ScriptableObject>> preloadCache = new();

    // 라벨 기반 로딩 캐시 (라벨 → 키 목록 → SO)
    private static readonly Dictionary<string, Dictionary<string, ScriptableObject>> labelCache = new();

    #region Preload 기반 로딩

    //명시적 단건 로딩
    public static async UniTask LoadAsync(Type type, string key)
    {
        if (!preloadCache.TryGetValue(type, out var cache))
        {
            cache = new Dictionary<string, ScriptableObject>();
            preloadCache[type] = cache;
        }

        if (cache.ContainsKey(key)) return;

        AsyncOperationHandle<ScriptableObject> handle = Addressables.LoadAssetAsync<ScriptableObject>(key);
        await handle.ToUniTask();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            ScriptableObject result = handle.Result;
            if (type.IsInstanceOfType(result))  //result가 매개변수 type과 같은 type인지 확인
            {
                cache[key] = result;
                Debug.Log($"[AddressableSO] Preloaded '{key}' as {type.Name}");
            }
            else
            {
                Debug.LogError($"[AddressableSO] Type mismatch: {key} is not {type.Name}");
            }
        }
        else
        {
            Debug.LogError($"[AddressableSO] Load failed: {key}");
        }
    }

    public static async UniTask LoadAsync(Type type, SOKey key)
    {
        await LoadAsync(type, SOKeys.Get(key));
    }

    //해당 type의 SO가 로드 완료되었는지 확인하는 함수
    public static bool IsLoaded(Type type, string key)
    {
        return preloadCache.TryGetValue(type, out var cache) && cache.ContainsKey(key);
    }

    public static bool IsLoaded(Type type, SOKey key)
    {
        return IsLoaded(type, SOKeys.Get(key));
    }

    //해당 type의 SO를 가져오는 함수
    public static ScriptableObject GetCached(Type type, string key)
    {
        if (preloadCache.TryGetValue(type, out var cache) && cache.TryGetValue(key, out var so))
            return so;
        return null;
    }

    public static ScriptableObject GetCached(Type type, SOKey key)
    {
        return GetCached(type, SOKeys.Get(key));
    }

    //해당 type의 SO를 언로드하는 함수
    public static void Unload(Type type, string key)
    {
        if (preloadCache.TryGetValue(type, out var cache) && cache.TryGetValue(key, out var so))
        {
            Addressables.Release(so);
            cache.Remove(key);
            Debug.Log($"[AddressableSO] Unloaded (Preload) - {key}");
        }
    }

    public static void Unload(Type type, SOKey key)
    {
        Unload(type, SOKeys.Get(key));
    }

    //캐시에 저장된 모든 preloadInfos에 해당하는 SO들을 언로드하는 함수
    public static void UnloadAllPreloaded()
    {
        foreach (var cache in preloadCache.Values)
        {
            foreach (var so in cache.Values)
                Addressables.Release(so);
        }

        preloadCache.Clear();
        Debug.Log("[AddressableSO] All preloaded assets unloaded.");
    }

    #endregion

    #region Label 기반 로딩

    //기능 단위 묶음 로딩
    public static async UniTask LoadByLabel(string label)
    {
        IList<IResourceLocation> locations = await Addressables.LoadResourceLocationsAsync(label).ToUniTask();

        if (!labelCache.TryGetValue(label, out var cache))
        {
            cache = new Dictionary<string, ScriptableObject>();
            labelCache[label] = cache;
        }

        foreach (IResourceLocation location in locations)
        {
            //location.ResourceType이 ScriptableObject를 상속 받았는지 확인
            if (!typeof(ScriptableObject).IsAssignableFrom(location.ResourceType))  
                continue;

            AsyncOperationHandle<ScriptableObject> handle = Addressables.LoadAssetAsync<ScriptableObject>(location);
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                ScriptableObject so = handle.Result;
                string key = so.name;

                if (!cache.ContainsKey(key))
                {
                    cache[key] = so;
                    Debug.Log($"[AddressableSO] Cached from label '{label}': {key}");
                }
            }
        }
    }

    //라벨과 type의 name에 해당하는 SO를 가져오는 함수
    public static ScriptableObject GetCachedByLabel(string label, string key)
    {
        if (labelCache.TryGetValue(label, out var cache) && cache.TryGetValue(key, out var so))
            return so;

        return null;
    }

    public static ScriptableObject GetCachedByLabel(string label, SOKey key)
    {
        return GetCachedByLabel(label, SOKeys.Get(key));
    }

    //매개변수로 받은 라벨에 해당하는 모든 SO를 언로드하는 함수
    public static void UnloadByLabel(string label)
    {
        if (!labelCache.TryGetValue(label, out var cache)) return;

        foreach (var so in cache.Values)
        {
            Addressables.Release(so);
        }

        labelCache.Remove(label);
        Debug.Log($"[AddressableSO] Unloaded all from label '{label}'");
    }

    //cache에 저장된 모든 라벨을 가진 SO들을 언로드하는 함수
    public static void UnloadAllLabels()
    {
        foreach (var cache in labelCache.Values)
        {
            foreach (var so in cache.Values)
            {
                Addressables.Release(so);
            }
        }

        labelCache.Clear();
        Debug.Log("[AddressableSO] All label-based assets unloaded.");
    }

    #endregion
}