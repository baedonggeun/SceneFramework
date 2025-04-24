using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class RequiredPluginRule : IPresetValidationRule
{
    public void Validate(ScenePresetSO preset, List<ScenePluginSO> plugins)
    {
        var allAvailable = AssetDatabase.FindAssets("t:ScenePluginSO")
            .Select(guid => AssetDatabase.LoadAssetAtPath<ScenePluginSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(p => p != null)
            .ToList();

        var targetScene = preset.SceneType;

        var requiredPlugins = allAvailable
            .Where(p => p.IsRequired && (p.TargetSceneType == targetScene || p.TargetSceneType == SceneType.Global));

        foreach (var required in requiredPlugins)
        {
            if (!plugins.Contains(required))
            {
                Debug.LogWarning($"[PresetValidation] {preset.name} ▶ 필수 플러그인 누락: {required.PluginName}");
            }
        }
    }
}
#endif
