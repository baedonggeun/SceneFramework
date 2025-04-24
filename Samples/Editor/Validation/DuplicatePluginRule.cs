using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
//참조 기반 중복 검사 
public class DuplicatePluginRule : IPresetValidationRule
{
    public void Validate(ScenePresetSO preset, List<ScenePluginSO> plugins)
    {
        // 동일 인스턴스 중복 감지
        var duplicateRefs = plugins
            .GroupBy(p => p)
            .Where(g => g.Count() > 1);

        foreach (var group in duplicateRefs)
        {
            Debug.LogWarning($"[PresetValidation] {preset.name} ▶ 중복된 플러그인 인스턴스: {group.Key.PluginName} ({group.Count()}회)");
        }

        // 동일 PluginName을 가진 서로 다른 인스턴스 감지
        var nameGroups = plugins
            .GroupBy(p => p.PluginName)
            .Where(g => g.Select(p => p.GetInstanceID()).Distinct().Count() > 1);

        foreach (var group in nameGroups)
        {
            Debug.LogWarning($"[PresetValidation] {preset.name} ▶ 같은 이름의 서로 다른 플러그인 감지: {group.Key} ({group.Count()}개 인스턴스)");
        }
    }
}
#endif