using System.Collections.Generic;

public static class SceneTransitionStatesList
{
    public static List<ISceneTransitionState> GetDefault()
    {
        return new List<ISceneTransitionState>
        {
            new CheckConditionState(),        // 1. 조건 검사
            new UnloadPreviousSceneState(),   // 2. 이전 씬 언로드
            new ShowLoadingSceneState(),      // 3. 로딩 씬 로드
            new LoadSceneState(),             // 4. 대상 씬 Additive 로드

            new LoadPresetState(),            // 5. 프리셋 로드
            new EnterSceneCompleteState(),    // 6. 후처리 (완료 로그, Cursor 등)

            new UnloadLoadingSceneState(),    // 7. 로딩 씬 언로드
            new IdleState()                   // 8. FSM 종료 후 대기
        };
    }
}