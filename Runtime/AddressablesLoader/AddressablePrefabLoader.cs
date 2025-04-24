using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class AddressablePrefabLoader
{
    private static readonly Dictionary<string, GameObject> prefabCache = new();

    private static readonly Dictionary<string, List<string>> labelToKeys = new();

    // 프리팹 비동기 로드 (Addressables 키로)
    public static async UniTask<GameObject> LoadAsync(string key)
    {
        if (prefabCache.ContainsKey(key))
            return prefabCache[key];

        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);
        await handle.ToUniTask();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefab = handle.Result;
            prefabCache[key] = prefab;
            return prefab;
        }

        Debug.LogError($"[AddressablePrefab] Failed to load: {key}");
        return null;
    }

    public static async UniTask<GameObject> LoadAsync(PrefabKey key)
    {
        return await LoadAsync(PrefabKeys.Get(key));
    }

    // 프리팹 비동기 로드 (Addressables 키로) - 라벨 기반
    public static async UniTask LoadByLabel(string label)
    {
        if (labelToKeys.ContainsKey(label)) return;

        IList<IResourceLocation> locations = await Addressables.LoadResourceLocationsAsync(label).ToUniTask();
        List<string> loadedKeys = new List<string>();

        foreach (IResourceLocation loc in locations)
        {
            if (loc.ResourceType != typeof(GameObject)) continue;

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(loc);
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = handle.Result;
                string key = prefab.name;

                if (!prefabCache.ContainsKey(key))
                {
                    prefabCache[key] = prefab;
                    loadedKeys.Add(key);
                    Debug.Log($"[AddressablePrefab] 라벨 '{label}' → 프리팹 로드: {key}");
                }
            }
        }

        labelToKeys[label] = loadedKeys;
    }

    // 캐싱된 프리팹 반환
    public static GameObject GetCached(string key)
    {
        prefabCache.TryGetValue(key, out GameObject prefab);
        return prefab;
    }

    //이미 Load된 프리팹을 즉시 Instantiate하고, DI 주입을 함께 수행
    public static GameObject InstantiateWithInject(string key)
    {
        var prefab = GetCached(key);
        if (prefab == null)
        {
            Debug.LogError($"[AddressablePrefab] No prefab cached for key: {key}");
            return null;
        }

        GameObject instance = Object.Instantiate(prefab);

        var registry = SceneServiceManager.CurrentRegistry;
        if (registry != null)
            InjectRunner.TryInjectAll(instance, registry.AsDictionary());

        return instance;
    }

    public static async UniTask<GameObject> InstantiateAsync(string key, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject prefab = await LoadAsync(key);
        if (prefab == null) return null;

        GameObject instance = Object.Instantiate(prefab, position, rotation, parent);

        var registry = SceneServiceManager.CurrentRegistry;
        if (registry != null)
            InjectRunner.TryInjectAll(instance, registry.AsDictionary());

        return instance;
    }

    public static async UniTask<GameObject> InstantiateAsync(PrefabKey key, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return await InstantiateAsync(PrefabKeys.Get(key), position, rotation, parent);
    }

    // 단일 프리팹 언로드
    public static void Unload(string key)
    {
        if (prefabCache.TryGetValue(key, out GameObject prefab))
        {
            Addressables.Release(prefab);
            prefabCache.Remove(key);
            Debug.Log($"[AddressablePrefab] Unloaded: {key}");
        }
    }

    // 모든 캐싱된 프리팹 언로드
    public static void UnloadAll()
    {
        foreach (GameObject prefab in prefabCache.Values)
        {
            Addressables.Release(prefab);
        }

        prefabCache.Clear();
        Debug.Log("[AddressablePrefab] Unloaded all cached prefabs.");
    }

    //addressables 라벨 기반 프리팹 자동 언로드
    public static void UnloadByLabel(string label)
    {
        if (!labelToKeys.TryGetValue(label, out List<string> keys)) return;

        foreach (string key in keys)
        {
            if (prefabCache.TryGetValue(key, out GameObject prefab))
            {
                Addressables.Release(prefab);
                prefabCache.Remove(key);
            }
        }

        labelToKeys.Remove(label);
        Debug.Log($"[AddressablePrefab] 라벨 '{label}' 프리팹들 언로드 완료");
    }

    //전체 라벨 프리팹 한꺼번에 언로드
    public static void UnloadAllLabels()
    {
        foreach (KeyValuePair<string, List<string>> pair in labelToKeys)
        {
            foreach (string key in pair.Value)
            {
                if (prefabCache.TryGetValue(key, out GameObject prefab))
                {
                    Addressables.Release(prefab);
                    prefabCache.Remove(key);
                }
            }
        }

        labelToKeys.Clear();
        Debug.Log("[AddressablePrefab] 모든 라벨 기반 프리팹 언로드 완료");
    }
}