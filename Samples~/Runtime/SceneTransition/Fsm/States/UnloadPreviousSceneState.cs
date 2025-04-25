using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UnloadPreviousSceneState : ISceneTransitionState
{
    public async UniTask Enter(SceneTransitionContext context)
    {
        if (!context.UnloadPrevious)
        {
            Debug.Log("[UnloadPreviousSceneState] UnloadPrevious 옵션이 꺼져 있어 언로드 생략");
            return;
        }

        // 프리셋 언로드
        if (context.PreviousPreset != null &&
            context.PreviousPreset.SceneType != SceneKey.Global)
        {
            Debug.Log($"[UnloadPreviousSceneState] 프리셋 언로드: {context.PreviousPreset.SceneName}");
            await ScenePluginExecutor.ExecuteOnUnload(context.PreviousPreset);
        }

        // 씬 언로드
        if (context.LoadedSceneInstance.HasValue)
        {
            Debug.Log("[UnloadPreviousSceneState] 씬 언로드 중...");
            await Addressables.UnloadSceneAsync(context.LoadedSceneInstance.Value);
            context.LoadedSceneInstance = null;
        }
    }
}