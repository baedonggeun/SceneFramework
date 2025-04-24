using UnityEngine;

[CreateAssetMenu(menuName = "HybridSceneFramework/Conditions/CheckFlag")]
public class CheckFlagConditionSO : SceneTransitionConditionBase
{
    public string flagName;

    public override bool Evaluate()
    {
        //return GameFlagManager.Instance.IsFlagTrue(flagName); //todo : flagmanager 구현 필요
        return false;
    }

    public override string GetDebugName()
    {
        return $"Flag({flagName})";
    }
}