using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnterSceneCompleteState : ISceneTransitionState
{
    public async UniTask Enter(SceneTransitionContext context)
    {
        Debug.Log($"[EnterSceneCompleteState] 씬 전환 완료: {context.TargetSceneName}");

        // 필요 시 후처리 작업 가능
        // 예: 커서 보이기, 사운드 재생 등
        Cursor.visible = true;

        await UniTask.Yield(); // UI 반영 등 한 프레임 대기
    }
}