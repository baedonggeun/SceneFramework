using System.Collections.Generic;

public static class SceneTransitionStatesList
{
    public static List<ISceneTransitionState> GetDefault()
    {
        return new List<ISceneTransitionState>
        {
            new CheckConditionState(),
            new ShowLoadingUIState(),
            new UnloadPreviousSceneState(),
            new LoadSceneState(),
            new LoadPresetState(),
            new HideLoadingUIState(),
            new EnterSceneCompleteState(),
            new IdleState()
        };
    }
}