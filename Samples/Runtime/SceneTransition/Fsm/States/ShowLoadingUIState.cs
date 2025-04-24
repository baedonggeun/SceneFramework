using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShowLoadingUIState : ISceneTransitionState
{
    private const string LoadingUIKey = "LoadingUI";

    public async UniTask Enter(SceneTransitionContext context)
    {
        var ui = await context.IUIManager.PrepareGlobalUIAsync<LoadingUI>(LoadingUIKey);
        if (ui == null)
            throw new System.Exception("LoadingUI 생성 실패");

        context.LoadingUIInstance = ui;
        context.IUIManager.ActivateGlobalUI(LoadingUIKey);

        ui.SetProgress(0f);
        Debug.Log("[ShowLoadingUIState] 로딩 UI 활성화 완료");
    }
}