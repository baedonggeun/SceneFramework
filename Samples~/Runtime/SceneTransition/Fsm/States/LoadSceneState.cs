using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class LoadSceneState : ISceneTransitionState
{
    const float SceneWeight = 0.3f;

    public async UniTask Enter(SceneTransitionContext context)
    {
        string presetKey = context.TargetSceneName;
        string sceneName;
        ScenePresetSO preset = context.TargetPreset;

        if (preset == null)
        {
            var presetHandle = Addressables.LoadAssetAsync<ScenePresetSO>(presetKey);
            await presetHandle.ToUniTask();
            if (presetHandle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                throw new System.Exception($"ScenePresetSO 로딩 실패: {presetKey}");
            preset = presetHandle.Result;
        }

        if (preset == null)
            throw new System.Exception($"[LoadSceneState] ScenePresetSO null 오류: {presetKey}");

        sceneName = preset.SceneName;

        // 실제 씬을 Additive 모드로 로드
        var sceneHandle = Addressables.LoadSceneAsync(
            sceneName,
            LoadSceneMode.Additive,
            activateOnLoad: true
        );

        // 로드 중 진행도 업데이트 (0 ~ SceneWeight)
        while (!sceneHandle.IsDone)
        {
            float progress = sceneHandle.PercentComplete * SceneWeight;
            context.UpdateLoadingProgress(progress);
            await UniTask.Yield();
        }

        if (sceneHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            context.LoadedSceneInstance = sceneHandle.Result;
            // 최소한 SceneWeight 만큼은 채워두기
            context.UpdateLoadingProgress(SceneWeight);
            Debug.Log($"[LoadSceneState] 씬 로드 성공: {sceneName}");
        }
        else
        {
            Debug.LogError($"[LoadSceneState] 씬 로드 실패: {sceneName}");
            throw new System.Exception($"씬 로딩 실패: {sceneName}");
        }
    }
}