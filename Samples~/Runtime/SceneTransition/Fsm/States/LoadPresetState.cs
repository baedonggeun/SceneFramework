using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadPresetState : ISceneTransitionState
{
    const float SceneWeight  = 0.3f;
    const float PresetWeight = 0.6f;

    public async UniTask Enter(SceneTransitionContext context)
    {
        if (context.ISceneServiceManager == null)
        {
            Debug.LogError("[LoadPresetState] SceneServiceManager가 null입니다.");
            throw new System.Exception("SceneServiceManager 누락");
        }
        if (context.TargetPreset == null)
        {
            Debug.LogError("[LoadPresetState] TargetPreset이 null입니다.");
            throw new System.Exception("TargetPreset 누락");
        }

        Debug.Log($"[LoadPresetState] 프리셋 로드 시작: {context.TargetPreset.SceneName}");

        // 실제 프리셋(플러그인) 로드
        await context.ISceneServiceManager.LoadScenePreset(context.TargetPreset);

        // DI 레지스트리 갱신
        context.InjectRegistry = SceneServiceManager.CurrentRegistry;

        // 진행도: SceneWeight + PresetWeight 만큼 채우기
        context.UpdateLoadingProgress(SceneWeight + PresetWeight);

        Debug.Log("[LoadPresetState] 프리셋 로드 완료");
    }
}