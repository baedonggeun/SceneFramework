using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "HybridSceneFramework/Plugins/UI")]
public class AddressableUIPluginSO : ScenePluginSO
{
    [Header("씬에 사용되는 UI 프리팹")]
    [SerializeField] private UIInfo[] uiPrefabs;
    public UIInfo[] UIPrefabs => uiPrefabs;

    [System.Serializable]
    public class UIInfo
    {
        [Header("UI 프리팹의 Addressables 키")]
        public UIKey uiKey;

        [Header("씬 진입 시 자동 생성 여부")]
        public bool autoShow = true;
    }

    // 씬 진입 시 자동으로 UI를 생성하고 화면에 표시
    public override async UniTask OnLoad(ScenePresetSO context, Dictionary<Type, object> instanceMap)
    {
        if (!instanceMap.TryGetValue(typeof(IUIManager), out var uiManagerObj) || uiManagerObj is not IUIManager uiManager)
        {
            Debug.LogError("[AddressableUIPluginSO] IUIManager not found in DI container");
            return;
        }

        foreach (var ui in uiPrefabs)
        {
            if (!ui.autoShow) continue;

            try
            {
                string uiName = UIKeys.Get(ui.uiKey);
                await uiManager.ShowByNameAsync(uiName);
                Debug.Log($"[AddressableUIPluginSO] Show UI: {uiName}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AddressableUIPluginSO] Failed to show UI: {ui.uiKey} - {ex.Message}");
            }
        }
    }

    // 씬 언로드 시 UI를 정리
    public override UniTask OnUnload(ScenePresetSO context)
    {
        if (uiPrefabs == null) return UniTask.CompletedTask;

        foreach (UIInfo ui in uiPrefabs)
        {
            UIManager.Instance.Hide(UIKeys.Get(ui.uiKey));
        }

        return UniTask.CompletedTask;
    }
}