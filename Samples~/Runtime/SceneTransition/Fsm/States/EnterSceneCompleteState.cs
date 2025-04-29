using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnterSceneCompleteState : ISceneTransitionState
{
    const float FullProgress = 1f;

    public async UniTask Enter(SceneTransitionContext context)
    {
        // 최종 진행도 완전 충전
        context.UpdateLoadingProgress(FullProgress);

        Debug.Log($"[EnterSceneCompleteState] 씬 전환 완료: {context.TargetSceneName}");

        // 후처리: 커서 보이기 등
        Cursor.visible = true;

        // UI 반영을 위해 한 프레임 대기
        await UniTask.Yield();
    }
}