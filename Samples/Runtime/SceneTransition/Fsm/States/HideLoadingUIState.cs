using Cysharp.Threading.Tasks;
using UnityEngine;

public class HideLoadingUIState : ISceneTransitionState
{
    private const string LoadingUIKey = "LoadingUI";

    public async UniTask Enter(SceneTransitionContext context)
    {
        context.IUIManager.DeactivateGlobalUI(LoadingUIKey);
        await UniTask.Yield();
    }
}