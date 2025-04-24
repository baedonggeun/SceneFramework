using UnityEngine;

[CreateAssetMenu(menuName = "HybridSceneFramework/Conditions/CheckHasItem")]
public class CheckHasItemConditionSO : SceneTransitionConditionBase
{
    public string itemID;

    public override bool Evaluate()
    {
        //return InventorySystem.Instance.HasItem(itemID);  //todo : inventorysystem 구현 필요
        return false;
    }

    public override string GetDebugName()
    {
        return $"HasItem({itemID})";
    }
}