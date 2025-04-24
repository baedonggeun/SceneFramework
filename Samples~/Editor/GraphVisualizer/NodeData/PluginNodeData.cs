using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets;

public class PluginNodeData : INodeData
{
    public ScenePluginSO Plugin { get; }
    public NodeType Type { get; }

    public string Label => $"[Plugin] {Plugin.PluginName}";

    public Dictionary<NodeType, string[]> ContentsByType { get; }

    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; } = new Vector2(300f, 200f);

    public Color Color => NodeColorUtil.GetColorForPlugin(Type);
    public UnityEngine.Object TargetObject => Plugin;

    public PluginNodeData(ScenePluginSO plugin)
    {
        Plugin = plugin;
        Type = GraphUtil.DetermineNodeType(plugin);
        ContentsByType = BuildContentsByType(plugin);
    }

    private Dictionary<NodeType, string[]> BuildContentsByType(ScenePluginSO plugin)
    {
        Dictionary<NodeType, List<string>> dict = new();
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        switch (plugin)
        {
            case ServicePluginSO servicePlugin:

                dict[NodeType.Service] = servicePlugin.RequiredServices
                    .Select(service =>
                    {
                        string tag = service.isSceneScoped ? "IsSceneScoped" : string.Empty;
                        return string.IsNullOrEmpty(tag)
                            ? $"{service.typeName}"
                            : $"{service.typeName} ({tag})";
                    })
                    .ToList();
                break;

            case AddressableSOPluginSO soPlugin:
                var soList = new List<string>();

                if (soPlugin.PreloadInfos != null)
                    soList.AddRange(soPlugin.PreloadInfos.Select(SO => $"{SO.key}"));

                if (settings != null && soPlugin.AddressableLabels != null)
                {
                    foreach (var label in soPlugin.AddressableLabels)
                    {
                        var entries = settings.groups.SelectMany(g => g.entries).Where(e => e.labels.Contains(label));
                        foreach (var entry in entries)
                        {
                            if (typeof(ScriptableObject).IsAssignableFrom(entry.MainAssetType) &&
                                entry.MainAssetType != typeof(ScenePresetSO))
                                soList.Add($"{entry.address} (label : {label})");
                        }
                    }
                }
                dict[NodeType.AddressableSO] = soList;
                break;

            case AddressableUIPluginSO uiPlugin:
                var uiList = new List<string>();
                Dictionary<string, string> uiKeyToLabel = new();
                if (settings != null)
                {
                    foreach (var group in settings.groups)
                    {
                        foreach (var entry in group.entries)
                        {
                            string key = entry.address;
                            if (!uiKeyToLabel.ContainsKey(key) && entry.labels.Count > 0)
                            {
                                uiKeyToLabel[key] = entry.labels.FirstOrDefault();
                            }
                        }
                    }
                }
                if (uiPlugin.UIPrefabs != null)
                {
                    uiList.AddRange(uiPlugin.UIPrefabs.Select(ui =>
                    {
                        string label = null;
                        uiKeyToLabel.TryGetValue(ui.uiKey.ToString(), out label);
                        return !string.IsNullOrEmpty(label)
                            ? $"{ui.uiKey} (label : {label})"
                            : $"{ui.uiKey}";
                    }));
                }

                dict[NodeType.AddressableUI] = uiList;
                break;

            case AddressablePrefabPluginSO prefabPlugin:
                var prefabList = new List<string>();
                Dictionary<string, string> prefabKeyToLabel = new();

                // 1. 라벨 기반으로 Addressables 그룹에서 프리팹 address → 라벨 매핑 생성
                if (settings != null && prefabPlugin.GameObjectPrefabLabels != null)
                {
                    foreach (var label in prefabPlugin.GameObjectPrefabLabels)
                    {
                        var entries = settings.groups.SelectMany(g => g.entries).Where(e => e.labels.Contains(label));
                        foreach (var entry in entries)
                        {
                            if (entry.MainAssetType == typeof(GameObject))
                            {
                                string key = entry.address;
                                if (!prefabKeyToLabel.ContainsKey(key))
                                    prefabKeyToLabel[key] = label;
                            }
                        }
                    }
                }

                if (prefabPlugin.GameObjectPrefabs != null)
                {
                    foreach (var p in prefabPlugin.GameObjectPrefabs)
                    {
                        string key = p.prefabKey.ToString();
                        if (!string.IsNullOrEmpty(key))
                        {
                            if (prefabKeyToLabel.TryGetValue(key, out var label))
                                prefabList.Add($"{key} (label : {label})");
                            else
                                prefabList.Add($"{key}");
                        }
                    }
                }

                foreach (var kvp in prefabKeyToLabel)
                {
                    string key = kvp.Key;
                    string label = kvp.Value;
                    bool alreadyIncluded = prefabList.Any(x => x.StartsWith(key));
                    if (!alreadyIncluded)
                    {
                        prefabList.Add($"{key} (label : {label})");
                    }
                }

                dict[NodeType.AddressablePrefab] = prefabList;
                break;
        }

        return dict.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
    }
}