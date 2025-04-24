using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

[CreateAssetMenu(menuName = "HybridSceneFramework/Plugins/SceneConditionPlugin")]
public class SceneConditionPluginSO : ScenePluginSO
{
    [Tooltip("이 씬이 유효하기 위한 조건 목록 (모두 true여야 함)")]
    public List<SceneTransitionConditionBase> requiredConditions;

    public override async UniTask OnLoad(ScenePresetSO context, Dictionary<Type, object> instanceMap)
    {
        await UniTask.Yield(); // 비동기 함수이므로 최소 Yield 필요

        if (!IsAllConditionsMet())
        {
            Debug.LogWarning($"[SceneConditionPluginSO] 조건 미충족 - {name}");
        }
        else
        {
            Debug.Log($"[SceneConditionPluginSO] 조건 충족 완료 - {name}");
        }
    }

    public override async UniTask OnUnload(ScenePresetSO context)
    {
        await UniTask.Yield(); // 조건은 상태 유지 없음 → 언로드 불필요하여 아무 작업 없이 종료
    }

    public bool IsAllConditionsMet()
    {
        if (requiredConditions == null || requiredConditions.Count == 0)
            return true;

        foreach (var condition in requiredConditions)
        {
            if (condition == null || !condition.Evaluate())
            {
                Debug.Log($"실패한 조건: {condition?.GetDebugName() ?? "null"}");
                return false;
            }
        }

        return true;
    }

    public string GetDebugName()
    {
        if (requiredConditions == null || requiredConditions.Count == 0)
            return "ConditionPlugin(empty)";

        return $"ConditionPlugin({string.Join(", ", requiredConditions.Select(c => c?.GetDebugName() ?? "null"))})";
    }
}