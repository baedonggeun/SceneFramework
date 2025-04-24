using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
//Preset 간 순환 참조 검사
public class SubPresetCycleRule : IPresetValidationRule
{
    public void Validate(ScenePresetSO preset, List<ScenePluginSO> plugins)
    {
        if (HasPresetCycle(preset))
        {
            Debug.LogError($"[PresetValidation] {preset.name} ▶ subPreset 간 순환 참조가 감지됨!");
        }
    }

    private bool HasPresetCycle(ScenePresetSO root)
    {
        HashSet<ScenePresetSO> visited = new();
        HashSet<ScenePresetSO> path = new();

        bool Visit(ScenePresetSO node)
        {
            if (node == null) return false;
            if (path.Contains(node)) return true;

            path.Add(node);
            foreach (var sub in node.SubPresets)
            {
                if (Visit(sub)) return true;
            }
            path.Remove(node);
            visited.Add(node);
            return false;
        }

        return Visit(root);
    }
}
#endif