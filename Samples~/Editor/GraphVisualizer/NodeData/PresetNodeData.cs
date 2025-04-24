using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PresetNodeData : INodeData
{
    public ScenePresetSO Preset { get; }

    public string Label => $"[Preset] {Preset.SceneName} ({Preset.SceneType})";

    public Dictionary<NodeType, string[]> ContentsByType { get; }

    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; } = new Vector2(300f, 200f);

    public Color Color => NodeColorUtil.GetColorForPreset(Preset.SceneType);
    public UnityEngine.Object TargetObject => Preset;

    public PresetNodeData(ScenePresetSO preset)
    {
        Preset = preset;
        ContentsByType = BuildContentsByType(preset);
    }

    private Dictionary<NodeType, string[]> BuildContentsByType(ScenePresetSO preset)
    {
        Dictionary<NodeType, List<string>> dict = new();

        if (preset.SubPresets != null && preset.SubPresets.Count > 0)
        {
            dict[NodeType.Preset] = preset.SubPresets
                .Select(p => p.SceneName)
                .ToList();
        }

        AddPlugins<ServicePluginSO>(preset, NodeType.Service, dict);
        AddPlugins<AddressableSOPluginSO>(preset, NodeType.AddressableSO, dict);
        AddPlugins<AddressableUIPluginSO>(preset, NodeType.AddressableUI, dict);
        AddPlugins<AddressablePrefabPluginSO>(preset, NodeType.AddressablePrefab, dict);

        return dict.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
    }

    private void AddPlugins<TPlugin>(ScenePresetSO preset, NodeType type, Dictionary<NodeType, List<string>> dict)
        where TPlugin : ScenePluginSO
    {
        var list = preset.Plugins
            .Where(p => p is TPlugin)
            .Select(p => p.PluginName)
            .ToList();

        if (list.Count > 0)
            dict[type] = list;
    }
}