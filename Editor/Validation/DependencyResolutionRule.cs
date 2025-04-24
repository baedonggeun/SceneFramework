using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
//종속성 누락 및 순서 오류 검사
public class DependencyResolutionRule : IPresetValidationRule
{
    public void Validate(ScenePresetSO preset, List<ScenePluginSO> plugins)
    {
        var injectables = plugins
            .OfType<IInjectable>()
            .ToList();

        try
        {
            InjectSortingUtil.TopologicalSort(injectables);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PresetValidation] {preset.name} ▶ 의존성 정렬 실패 또는 순환 의존성 존재\n↳ {ex.Message}");
        }

        foreach (var injectable in injectables)
        {
            foreach (var depType in injectable.GetDependencies())
            {
                bool found = plugins.Any(p => depType.IsAssignableFrom(p.GetType()));

                if (!found)
                {
                    Debug.LogError($"[PresetValidation] {preset.name} ▶ {injectable.GetType().Name}의 종속 타입 누락: {depType.FullName}");
                }
            }
        }
    }
}
#endif