using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadPresetState : ISceneTransitionState
{
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

        // 실제 로드
        await context.ISceneServiceManager.LoadScenePreset(context.TargetPreset);

        // InjectRegistry 캐싱
        context.InjectRegistry = SceneServiceManager.CurrentRegistry;

        // 로딩 진행도 완료
        context.UpdateLoadingProgress(1f);

        Debug.Log("[LoadPresetState] 프리셋 로드 완료");
    }
}