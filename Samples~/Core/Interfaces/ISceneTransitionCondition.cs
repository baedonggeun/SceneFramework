public interface ISceneTransitionCondition
{
    bool Evaluate();                 // 조건 체크
    string GetDebugName();          // 디버그 로깅용
}