using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(menuName = "HybridSceneFramework/Plugins/AddressableSO")]
public class AddressableSOPluginSO : ScenePluginSO
{
    [Header("명시적으로 로드할 SO들 (key + typeName)")]
    [SerializeField] private PreloadInfo[] preloadInfos;

    [Header("Addressables 라벨 단위로 묶은 SO 그룹")]
    [SerializeField] private string[] addressableLabels;

    public PreloadInfo[] PreloadInfos => preloadInfos;
    public string[] AddressableLabels => addressableLabels;

    [Serializable]
    public class PreloadInfo
    {
        public SOKey key;
        public string typeName;
    }

    // 현재 씬이 로드될 때 호출됨
    public override async UniTask OnLoad(ScenePresetSO context, Dictionary<Type, object> instanceMap)
    {
        Debug.Log("[AddressableSOPluginSO] OnLoad");

        // 해당 씬에서 사용할 프리팹들을 미리 로드
        if (preloadInfos != null)
        {
            foreach (PreloadInfo info in preloadInfos)
            {
                Type type = Type.GetType(info.typeName);
                if (type == null)
                {
                    Debug.LogError($"[AddressableSOPlugin] 잘못된 타입명: {info.typeName}");
                    continue;
                }

                await AddressableSOLoader.LoadAsync(type, SOKeys.Get(info.key));
            }
        }

        // 특정 라벨로 묶인 프리팹들을 한 번에 로드
        if (addressableLabels != null)
        {
            foreach (string label in addressableLabels)
            {
                await AddressableSOLoader.LoadByLabel(label);
            }
        }
    }

    //현재 씬이 언로드 되거나 다른 씬으로 전환될 때 호출됨
    public override UniTask OnUnload(ScenePresetSO context)
    {
        Debug.Log("[AddressableSOPluginSO] OnUnload");

        // 미리 로드된 프리팹들을 언로드
        if (preloadInfos != null)
        {
            foreach (PreloadInfo info in preloadInfos)
            {
                Type type = Type.GetType(info.typeName);
                if (type == null) continue;

                AddressableSOLoader.Unload(type, SOKeys.Get(info.key));
            }
        }

        // 특정 라벨로 묶인 프리팹들을 언로드
        if (addressableLabels != null)
        {
            foreach (string label in addressableLabels)
            {
                AddressableSOLoader.UnloadByLabel(label);
            }
        }

        return UniTask.CompletedTask;
    }
}