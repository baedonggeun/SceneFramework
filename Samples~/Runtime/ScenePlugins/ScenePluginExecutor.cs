/*
=========================================
ScenePreset 병합 정책 요약 (Policy Summary)
=========================================

1. 중복 플러그인:
   - 참조가 같으면 1회만 병합
   - PluginName만 같고 참조가 다르면 경고 출력

2. 플러그인 정렬 순서:
   - 의존성 기반 위상 정렬
   - 종속성 없음 시 subPreset 순서 → 알파벳 순 tie-break

3. 의존성 누락:
   - 병합된 플러그인 목록에 없는 DependsOn 대상은 오류 처리
   - IsRequired가 true일 경우 검증 실패 처리

4. 병합 순서:
   - subPresets 순서대로 병합
   - 병합 순서가 실행 순서에 영향

5. 순환 참조:
   - subPreset 내에 자기 자신 포함 시 오류
   - Validation 도구에서 자동 검출

(자세한 내용은 Docs/SceneFramework/PluginMergePolicy.md 참고)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

public static class ScenePluginExecutor
{
    private static readonly HashSet<ScenePluginSO> globalLoadedPlugins = new();

    // 병합된 전체 플러그인 리스트를 의존성과 우선순위 기준으로 정렬하여 순차 로드
    public static async UniTask ExecuteOnLoad(ScenePresetSO preset, InjectRegistry registry)
    {
        var rawPlugins = preset.GetAllPlugins()
            .Where(p => p != null)
            .Distinct()
            .ToList();

        var sortedPlugins = SortByPluginTypeAndPriority(rawPlugins);

        var instanceMap = registry.AsDictionary();

        foreach (var plugin in sortedPlugins)
        {
            bool isGlobal = preset.SceneType == SceneKey.Global;

            if (isGlobal && globalLoadedPlugins.Contains(plugin)) continue;

            try
            {
                await plugin.OnLoad(preset, instanceMap);
                Debug.Log($"[PluginExecutor:OnLoad] {plugin.PluginName} 로드 완료");

                if (isGlobal)
                    globalLoadedPlugins.Add(plugin);
            }
            catch (Exception ex)
            { Debug.LogError($"[PluginExecutor:OnLoad] {plugin.PluginName} 실패: {ex.Message}"); }
        }
    }

    // 병합된 전체 플러그인 리스트를 역순으로 정렬하여 언로드
    public static async UniTask ExecuteOnUnload(ScenePresetSO preset)
    {
        if (preset.SceneType == SceneKey.Global)
            return;

        var plugins = preset.GetAllPlugins().Where(p => p != null).Reverse();

        foreach (var plugin in plugins)
        {
            try 
            { await plugin.OnUnload(preset); }
            catch (Exception ex)
            { Debug.LogError($"[PluginExecutor:OnUnload] {plugin.PluginName} 실패: {ex.Message}"); }
        }
    }

    // 플러그인 타입 기반 우선순위 + 사용자 Priority 기반 정렬
    public static List<ScenePluginSO> SortByPluginTypeAndPriority(List<ScenePluginSO> plugins)
    {
        return plugins
            .OrderBy(GetPluginTypePriority)
            .ThenBy(p => p.Priority)
            .ToList();
    }

    // 타입 기반 정렬 우선순위 정의 - 낮을수록 먼저 실행됨
    private static int GetPluginTypePriority(ScenePluginSO plugin)
    {
        return plugin switch
        {
            AddressableSOPluginSO => 0,
            ServicePluginSO => 1,
            AddressableUIPluginSO => 2,
            AddressablePrefabPluginSO => 3,
            _ => 10
        };
    }
}