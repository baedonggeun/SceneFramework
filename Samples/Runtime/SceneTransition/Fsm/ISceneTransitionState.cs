using Cysharp.Threading.Tasks;

// TransitionFSM의 각 단일 상태를 정의하는 인터페이스
public interface ISceneTransitionState
{
    // 상태 진입 시 호출
    UniTask Enter(SceneTransitionContext context);
}