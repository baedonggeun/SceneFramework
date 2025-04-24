using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
//IInitializable을 구현했지만 AutoInitialize == false인 플러그인 검사
public class AutoInitializeSkippedRule : IPresetValidationRule
{
    public void Validate(ScenePresetSO preset, List<ScenePluginSO> plugins)
    {
        foreach (var plugin in plugins)
        {
            if (plugin is IInitializable init && !init.AutoInitialize)
            {
                Debug.LogWarning($"[PresetValidation] {preset.name} ▶ AutoInitialize 비활성화: {plugin.name} ({plugin.GetType().Name})");
            }
        }
    }
}
#endif