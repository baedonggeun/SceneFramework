using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UnloadLoadingSceneState : ISceneTransitionState
{
    public async UniTask Enter(SceneTransitionContext context)
    {
        if (context.LoadingSceneInstance.HasValue)
        {
            await Addressables.UnloadSceneAsync(context.LoadingSceneInstance.Value);
            context.LoadingSceneInstance = null;
            Debug.Log("[UnloadLoadingSceneState] 로딩 씬 언로드 완료");
        }
    }
}