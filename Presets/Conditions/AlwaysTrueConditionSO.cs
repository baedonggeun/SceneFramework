using UnityEngine;

[CreateAssetMenu(menuName = "HybridSceneFramework/Conditions/AlwaysTrue")]
public class AlwaysTrueConditionSO : SceneTransitionConditionBase
{
    public override bool Evaluate() => true;

    public override string GetDebugName() => "AlwaysTrue";
}