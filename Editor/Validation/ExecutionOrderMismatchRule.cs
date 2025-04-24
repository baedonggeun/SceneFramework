using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
//DI 기반 정렬 결과와 Priority 기반 정렬 결과가 불일치 검사
public class ExecutionOrderMismatchRule : IPresetValidationRule
{
    public void Validate(ScenePresetSO preset, List<ScenePluginSO> plugins)
    {
        // Priority + PluginType 정렬 기준
        var prioritySorted = ScenePluginExecutor.SortByPluginTypeAndPriority(plugins);

        // Injectable 위상 정렬 기준
        var injectables = plugins.OfType<IInjectable>().ToList();

        List<IInjectable> injectableSorted;
        try
        {
            injectableSorted = InjectSortingUtil.TopologicalSort(injectables);
        }
        catch
        {
            // 정렬 실패 시는 DependencyResolutionRule이 담당하므로 이쪽은 생략
            return;
        }

        // 정렬 순서 비교
        for (int i = 0; i < Mathf.Min(prioritySorted.Count, injectableSorted.Count); i++)
        {
            var prio = prioritySorted[i];
            var inj = injectableSorted[i];

            if (prio.GetType() == inj.GetType())
            {
                int prioID = prio.GetInstanceID();
                int injID = (inj as ScriptableObject)?.GetInstanceID() ?? -1;

                if (prioID != injID)
                {
                    Debug.LogWarning($"[PresetValidation] {preset.name} ▶ 정렬 기준 불일치 감지: {prio.name} vs {inj.GetType().Name}");
                }
            }
        }
    }
}
#endif