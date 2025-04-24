#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScenePresetAutoBuilderWindow : EditorWindow
{
    private ScenePresetSO targetPreset;
    private ScenePresetSO previousPreset;
    private SceneMatchRuleSO matchRule;
    private List<ScenePluginSO> discoveredPlugins = new();
    private Vector2 scroll;
    private bool clearExisting = true;

    [MenuItem("Tools/Hybrid Scene Framework/Auto Build ScenePreset")]
    public static void ShowWindow()
    {
        GetWindow<ScenePresetAutoBuilderWindow>("Auto Build Preset");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("ScenePreset 자동 플러그인 등록기", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        var selectedPreset = (ScenePresetSO)EditorGUILayout.ObjectField("Target Preset", targetPreset, typeof(ScenePresetSO), false);
        matchRule = (SceneMatchRuleSO)EditorGUILayout.ObjectField("Scene Match Rule", matchRule, typeof(SceneMatchRuleSO), false);

        // Preset 변경 감지 시 자동 초기화
        if (selectedPreset != targetPreset)
        {
            targetPreset = selectedPreset;
            discoveredPlugins.Clear();
            scroll = Vector2.zero;
        }

        if (GUILayout.Button("플러그인 탐색 및 필터링"))
        {
            DiscoverAndFilterPlugins();
        }

        if (discoveredPlugins.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("자동 정렬된 플러그인 리스트", EditorStyles.boldLabel);
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(200));
            foreach (var plugin in discoveredPlugins)
                EditorGUILayout.LabelField($"{plugin.PluginName} ({plugin.TargetSceneType}, Priority: {plugin.Priority})");
            EditorGUILayout.EndScrollView();
        }

        clearExisting = EditorGUILayout.ToggleLeft("기존 플러그인을 삭제하고 덮어쓰기", clearExisting);

        if (targetPreset != null && discoveredPlugins.Count > 0 && GUILayout.Button(" Preset에 적용"))
        {
            ApplyPluginsToPreset();
        }
    }

    private void DiscoverAndFilterPlugins()
    {
        var allPluginGUIDs = AssetDatabase.FindAssets("t:ScenePluginSO");
        var allPlugins = allPluginGUIDs
            .Select(guid => AssetDatabase.LoadAssetAtPath<ScenePluginSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(p => p != null)
            .ToList();

        if (targetPreset == null || matchRule == null)
        {
            Debug.LogWarning("Preset 또는 MatchRuleSO를 지정해주세요.");
            return;
        }

        var sceneType = matchRule.GetSceneType(targetPreset.SceneName);

        var filtered = allPlugins
            .Where(p => p.TargetSceneType == sceneType || p.TargetSceneType == SceneType.Global)
            .ToList();

        discoveredPlugins = SortByDependencyAndPriority(filtered);
    }

    private void ApplyPluginsToPreset()
    {
        Undo.RecordObject(targetPreset, "Apply Plugins");

        if (clearExisting)
        {
            targetPreset.Plugins = discoveredPlugins;
        }
        else
        {
            var existing = targetPreset.Plugins ?? new List<ScenePluginSO>();
            var merged = existing.Union(discoveredPlugins).ToList();
            targetPreset.Plugins = SortByDependencyAndPriority(merged.Distinct().ToList());
        }

        EditorUtility.SetDirty(targetPreset);
        AssetDatabase.SaveAssets();

        Debug.Log($"[ScenePresetAutoBuilder] {targetPreset.name}에 플러그인 {(clearExisting ? "덮어쓰기" : "병합")} 완료.");
    }

    private static List<ScenePluginSO> SortByDependencyAndPriority(List<ScenePluginSO> plugins)
    {
        Dictionary<string, ScenePluginSO> nameToPlugin = plugins.ToDictionary(p => p.PluginName);
        HashSet<string> visited = new();
        List<ScenePluginSO> result = new();

        void Visit(ScenePluginSO plugin)
        {
            if (visited.Contains(plugin.PluginName)) return;

            //foreach (ScenePluginSO dep in plugin.DependsOn)
            //{
            //    if (dep == null) continue;

            //    if (nameToPlugin.TryGetValue(dep.PluginName, out var depPlugin))
            //        Visit(depPlugin);
            //    else
            //        Debug.LogWarning($"[AutoBuilder] {plugin.PluginName} → 의존성 누락: {dep.PluginName}");
            //}

            visited.Add(plugin.PluginName);
            result.Add(plugin);
        }

        foreach (var plugin in plugins.OrderBy(p => p.Priority))
        {
            Visit(plugin);
        }

        return result;
    }
}
#endif