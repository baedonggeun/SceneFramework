using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using System.Collections.Generic;

public class SceneTransitionManager : MonoSingleton<SceneTransitionManager>, ISceneTransitionManager, IInitializable, IInjectable
{
    public int Priority => 1;
    public bool AutoInitialize => true;

    [Inject] private ISceneServiceManager _sceneServiceManager;
    [Inject] private IUIManager _uiManager;

    public Type[] GetDependencies() => new Type[] { typeof(ISceneServiceManager), typeof(IUIManager) };

    private string currentSceneName;
    private SceneInstance? loadedSceneInstance;
    private ScenePresetSO previousPreset;

    public async UniTask InitializeAsync()
    {
        await UniTask.Yield();
    }

    public async UniTask<bool> TryTransition(string targetScene, bool additive = false, bool unloadCurrent = true)
    {
        Debug.Log($"[SceneTransition] TryTransition: {targetScene}");

        //1. FSM Context 구성
        var context = SceneTransitionContext.Create(
            targetSceneName: targetScene,
            uiManager: _uiManager,
            sceneServiceManager: _sceneServiceManager,
            registry: SceneServiceManager.CurrentRegistry,
            additive: additive,
            unloadPrevious: unloadCurrent
        );

        context.PreviousPreset = previousPreset;
        context.LoadedSceneInstance = loadedSceneInstance;

        //2. FSM 구성
        var fsm = new SceneTransitionFSM(SceneTransitionStatesList.GetDefault());

        //3. FSM 실행
        try
        {
            await fsm.Run(context);

            //4. 성공 시 내부 상태 갱신
            currentSceneName = targetScene;
            loadedSceneInstance = context.LoadedSceneInstance;
            previousPreset = context.TargetPreset;

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SceneTransition] 전환 실패: {ex.Message}");
            return false;
        }
    }
}