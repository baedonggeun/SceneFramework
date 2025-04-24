using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "HybridSceneFramework/Plugins/AddressablePrefab")]
public class AddressablePrefabPluginSO : ScenePluginSO
{
    [Header("개별 프리팹 addressable 키")]
    [SerializeField] private PrefabInfo[] gameObjectPrefabs;

    [Header ("로드할 프리팹들의 label")]
    [SerializeField] private string[] gameObjectPrefabLabels;

    public PrefabInfo[] GameObjectPrefabs => gameObjectPrefabs;
    public string[] GameObjectPrefabLabels => gameObjectPrefabLabels;

    [Serializable]
    public class PrefabInfo
    {
        [Header("프리팹 Addressable 키")]
        public PrefabKey prefabKey;
    }

    public override async UniTask OnLoad(ScenePresetSO context, Dictionary<Type, object> instanceMap)
    {
        if (gameObjectPrefabs != null)
        {
            foreach (var prefab in gameObjectPrefabs)
            {
                string key = prefab.prefabKey.ToString();
                if (string.IsNullOrEmpty(key)) continue;

                await AddressablePrefabLoader.LoadAsync(prefab.prefabKey);
            }
        }

        if (gameObjectPrefabLabels != null)
        {
            foreach (var label in gameObjectPrefabLabels)
            {
                await AddressablePrefabLoader.LoadByLabel(label);
            }
        }
    }

    public override UniTask OnUnload(ScenePresetSO context)
    {
        Debug.Log("[AddressablePrefabPluginSO] OnUnload");

        if (gameObjectPrefabs != null)
        {
            foreach (var prefab in gameObjectPrefabs)
            {
                if (string.IsNullOrEmpty(PrefabKeys.Get(prefab.prefabKey))) continue;
                AddressablePrefabLoader.Unload(PrefabKeys.Get(prefab.prefabKey));
            }
        }

        if (gameObjectPrefabLabels != null)
        {
            foreach (var label in gameObjectPrefabLabels)
            {
                AddressablePrefabLoader.UnloadByLabel(label);
            }
        }

        return UniTask.CompletedTask;
    }
}