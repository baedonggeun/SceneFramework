using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class LoadSceneState : ISceneTransitionState
{
    public async UniTask Enter(SceneTransitionContext context)
    {
        string sceneName = context.TargetSceneName;
        LoadSceneMode mode = context.Additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

        var handle = Addressables.LoadSceneAsync(sceneName, mode, activateOnLoad: true);

        while (!handle.IsDone)
        {
            context.UpdateLoadingProgress(handle.PercentComplete);
            await UniTask.Yield();
        }

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            context.LoadedSceneInstance = handle.Result;
            Debug.Log($"[LoadSceneState] 씬 로드 성공: {sceneName}");
        }
        else
        {
            Debug.LogError($"[LoadSceneState] 씬 로드 실패: {sceneName}");
            throw new System.Exception($"씬 로딩 실패: {sceneName}");
        }
    }
}