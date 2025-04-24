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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public static class PresetValidationTool
{
    private static readonly List<IPresetValidationRule> Rules = new()
    {
        new RequiredPluginRule(),
        new DependencyResolutionRule(),
        new DuplicatePluginRule(),
        new SubPresetCycleRule(),

        new ExecutionOrderMismatchRule(),
        new AutoInitializeSkippedRule(),
        new MissingInterfaceBindingRule(),
        //필요한 경우 여기에 다른 Rule 추가
    };

    [MenuItem("Tools/Hybrid Scene Framework/Validate All ScenePresets")]
    public static void ValidateAllPresets()
    {
        var presetGUIDs = AssetDatabase.FindAssets("t:ScenePresetSO");
        var presets = presetGUIDs
            .Select(guid => AssetDatabase.LoadAssetAtPath<ScenePresetSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(p => p != null)
            .ToList();

        Debug.Log($"[PresetValidation] 전체 Preset 수: {presets.Count}");

        foreach (var preset in presets)
        {
            var allPlugins = preset.GetAllPlugins();
            Debug.Log($"[PresetValidation] 검사 시작 ▶ {preset.name}");

            foreach (var rule in Rules)
            {
                try
                {
                    rule.Validate(preset, allPlugins);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[PresetValidation] {preset.name} ▶ 룰 실행 중 예외 발생: {rule.GetType().Name}\n↳ {ex.Message}");
                }
            }

            Debug.Log($"[PresetValidation] 검사 완료 ▶ {preset.name}");
        }

        Debug.Log(" 모든 ScenePresetSO 유효성 검사 완료.");
    }
}
#endif