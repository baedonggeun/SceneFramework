using UnityEngine.ResourceManagement.ResourceProviders;

public class SceneTransitionContext
{
    // 전환 설정
    public string TargetSceneName { get; set; }
    public bool Additive { get; set; }
    public bool UnloadPrevious { get; set; }

    // 씬 및 프리셋 정보
    public ScenePresetSO TargetPreset { get; set; }
    public ScenePresetSO PreviousPreset { get; set; }
    public SceneInstance? LoadedSceneInstance { get; set; }

    // 로딩 UI
    public LoadingUI LoadingUIInstance { get; set; }
    public SceneInstance? LoadingSceneInstance { get; set; }

    // 필수 매니저
    public IUIManager IUIManager { get; set; }
    public ISceneServiceManager ISceneServiceManager { get; set; }

    // DI 레지스트리 (선택적)
    public InjectRegistry InjectRegistry { get; set; }

    // 진행도 갱신 헬퍼
    public void UpdateLoadingProgress(float progress)
    {
        LoadingUIInstance?.SetProgress(progress);
    }

    public static SceneTransitionContext Create(
        string targetSceneName,
        IUIManager uiManager,
        ISceneServiceManager sceneServiceManager,
        InjectRegistry registry,
        bool additive = false,
        bool unloadPrevious = true
    )
    {
        return new SceneTransitionContext
        {
            TargetSceneName = targetSceneName,
            IUIManager = uiManager,
            ISceneServiceManager = sceneServiceManager,
            InjectRegistry = registry,
            Additive = additive,
            UnloadPrevious = unloadPrevious
        };
    }
}