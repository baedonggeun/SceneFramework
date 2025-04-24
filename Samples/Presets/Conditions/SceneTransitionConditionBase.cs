using UnityEngine;

public abstract class SceneTransitionConditionBase : ScriptableObject, ISceneTransitionCondition
{
    public abstract bool Evaluate();
    public abstract string GetDebugName();
}