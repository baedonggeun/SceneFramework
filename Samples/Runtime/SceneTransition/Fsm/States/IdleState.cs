using Cysharp.Threading.Tasks;
using UnityEngine;

public class IdleState : ISceneTransitionState
{
    public async UniTask Enter(SceneTransitionContext context)
    {
        Debug.Log("[IdleState] 씬 전환 FSM 종료. 시스템 대기 상태 진입.");

        // 필요 시 전환 완료 이벤트 전송, 후처리 삽입 가능
        // Example:
        // EventBus.Raise(new SceneTransitionCompleteEvent(context.TargetSceneName));

        await UniTask.Yield(); // 구조 안정용 프레임 대기
    }
}