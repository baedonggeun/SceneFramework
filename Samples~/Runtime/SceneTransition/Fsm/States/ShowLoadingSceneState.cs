using Cysharp.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class ShowLoadingSceneState : ISceneTransitionState
{
    private string LoadingSceneKey = SceneKey.Loading.ToString();

    public async UniTask Enter(SceneTransitionContext context)
    {
        Debug.Log($"[ShowLoadingSceneState] Enter → 로딩 씬 로드 시작: {LoadingSceneKey}");

        Cursor.visible = false;

        // 싱글 모드로 로딩 씬만 남기기
        var handle = Addressables.LoadSceneAsync(
            LoadingSceneKey,
            LoadSceneMode.Single,
            activateOnLoad: true
        );
        await handle.ToUniTask();

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[ShowLoadingSceneState] 로딩 씬 로드 실패: {LoadingSceneKey}");
            throw new System.Exception($"로딩 씬 로드 실패: {LoadingSceneKey}");
        }

        // SceneInstance 저장
        context.LoadingSceneInstance = handle.Result;
        Debug.Log("[ShowLoadingSceneState] 로딩 씬 로드 완료");

        // 로딩 씬 내에 배치된 LoadingUI 컴포넌트 찾기
        var scene = handle.Result.Scene;
        LoadingUI loadingUI = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            loadingUI = root.GetComponentInChildren<LoadingUI>(true);
            if (loadingUI != null)
                break;
        }

        if (loadingUI == null)
        {
            Debug.LogError("[ShowLoadingSceneState] LoadingUI 컴포넌트를 찾을 수 없습니다!");
        }
        else
        {
            context.LoadingUIInstance = loadingUI;
            Debug.Log($"[ShowLoadingSceneState] LoadingUIInstance 할당: {loadingUI.name}");

            loadingUI.Opened();
            Debug.Log("[ShowLoadingSceneState] LoadingUI.Opened() 호출 완료");
        }

        // 초기 진행도 0 설정 (로그도 찍힘)
        context.UpdateLoadingProgress(0f);
    }
}