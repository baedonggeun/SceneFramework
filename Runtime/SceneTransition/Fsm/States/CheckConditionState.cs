using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CheckConditionState : ISceneTransitionState
{
    public async UniTask Enter(SceneTransitionContext context)
    {
        string sceneKey = context.TargetSceneName;

        var handle = Addressables.LoadAssetAsync<ScenePresetSO>(sceneKey);
        await handle.ToUniTask();

        if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[CheckConditionState] ScenePresetSO 로딩 실패: {sceneKey}");
            throw new System.Exception($"Preset 로드 실패: {sceneKey}");
        }

        ScenePresetSO preset = handle.Result;
        context.TargetPreset = preset;

        var conditionPlugin = preset.GetConditionPlugin();

        if (conditionPlugin != null && !conditionPlugin.IsAllConditionsMet())
        {
            Debug.LogWarning($"[CheckConditionState] 조건 미충족 → 전환 중단: {conditionPlugin.GetDebugName()}");
            throw new System.Exception("씬 전환 조건 미충족");
        }

        Debug.Log("[CheckConditionState] 조건 통과 및 Preset 로드 완료");
    }
}