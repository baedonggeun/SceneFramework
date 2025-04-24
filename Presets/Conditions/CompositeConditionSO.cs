using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum CompositeLogicType
{
    AND,
    OR,
    NOT
}

[CreateAssetMenu(menuName = "HybridSceneFramework/Conditions/Composite")]
public class CompositeConditionSO : SceneTransitionConditionBase
{
    [Tooltip("AND / OR / NOT 연산 중 하나 선택")]
    public CompositeLogicType logicType;

    [Tooltip("조건 목록 (NOT인 경우 첫 번째 조건만 사용)")]
    public List<SceneTransitionConditionBase> subConditions = new List<SceneTransitionConditionBase>
    {
        null // 기본값으로 AlwaysTrueConditionSO를 에디터에서 할당 권장
    };

    public override bool Evaluate()
    {
        if (subConditions == null || subConditions.Count == 0)
            return true; // 기본적으로 조건이 없으면 항상 true

        switch (logicType)
        {
            case CompositeLogicType.AND:
                return subConditions.All(c => c != null && c.Evaluate());

            case CompositeLogicType.OR:
                return subConditions.Any(c => c != null && c.Evaluate());

            case CompositeLogicType.NOT:
                var first = subConditions[0];
                return first != null && !first.Evaluate();

            default:
                return false;
        }
    }

    public override string GetDebugName()
    {
        if (subConditions == null || subConditions.Count == 0)
            return $"{logicType}(empty)";

        switch (logicType)
        {
            case CompositeLogicType.NOT:
                return $"NOT({subConditions[0]?.GetDebugName() ?? "null"})";

            case CompositeLogicType.AND:
                return $"AND({string.Join(" && ", subConditions.Select(c => c?.GetDebugName() ?? "null"))})";

            case CompositeLogicType.OR:
                return $"OR({string.Join(" || ", subConditions.Select(c => c?.GetDebugName() ?? "null"))})";

            default:
                return "UNKNOWN";
        }
    }
}
