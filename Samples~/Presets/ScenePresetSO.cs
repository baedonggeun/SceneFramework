/*
=========================================
ScenePreset 병합 정책 요약 (Policy Summary)
=========================================

1. 중복 플러그인:
   - 참조가 같으면 1회만 실행
   - PluginName만 같고 참조가 다르면 경고 출력

2. 우선순위(Priority) 충돌:
   - subPreset 병합 순서 → 알파벳 순으로 tie-break

3. 의존성 누락:
   - GetAllPlugins 결과에 없는 DependsOn 대상은 실행되지 않음
   - Required 플러그인일 경우 오류 처리

4. 병합 순서:
   - subPresets 순서대로 병합되며, 실행에도 영향 미침

5. 순환 참조:
   - subPreset 내에 자기 자신 포함 시 오류
   - Validation 도구에서 자동 검출 예정

(자세한 내용은 Docs/SceneFramework/PluginMergePolicy.md 참고)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "ScenePreset", menuName = "HybridSceneFramework/ScenePreset")]
public class ScenePresetSO : ScriptableObject
{
    [Header("씬 이름 (에디터용 메타 정보)")]
    [SerializeField] private string sceneName;

    [Header("씬 타입")]
    [SerializeField] private SceneKey sceneType;

    [Header("씬 전환 시 자동 언로드 여부")]
    [SerializeField] private bool autoUnloadOnSceneChange = true;

    [Header("이 Preset에 병합될 다른 Preset들")]
    [SerializeField] private List<ScenePresetSO> subPresets = new();

    [Header("이 ScenePreset에서 직접 사용하는 플러그인들")]
    [SerializeField] private List<ScenePluginSO> plugins = new();

    public List<ScenePluginSO> Plugins
    {
        get => plugins;
        set => plugins = value;
    }
    public List<ScenePresetSO> SubPresets
    {
        get => subPresets;
        set => subPresets = value;
    }
    public string SceneName
    {
        get => sceneName;
        set => sceneName = value;
    }
    public bool AutoUnloadOnSceneChange
    {
        get => autoUnloadOnSceneChange;
        set => autoUnloadOnSceneChange = value;
    }

    public SceneKey SceneType
    {
        get => sceneType;
        set => sceneType = value;
    }

    public List<ScenePluginSO> GetAllPlugins()
    {
        HashSet<ScenePluginSO> visited = new();           // 중복 제거용
        List<ScenePluginSO> result = new();                // 최종 반환 리스트

        HashSet<ScenePresetSO> presetVisited = new(); //순환 방지

        void Collect(ScenePresetSO preset)
        {
            if (preset == null) return;
            if (presetVisited.Contains(preset)) return; //순환 방지
            presetVisited.Add(preset);

            // 1. SubPreset 먼저 재귀 호출
            foreach (var sub in preset.subPresets)
                Collect(sub);

            // 2. 본인의 플러그인 추가 (중복 제거)
            foreach (var plugin in preset.plugins)
            {
                if (plugin == null) continue;
                if (visited.Contains(plugin)) continue;

                visited.Add(plugin);
                result.Add(plugin);
            }
        }

        // 시작: 현재 프리셋 기준
        Collect(this);

        return result;
    }

    public SceneConditionPluginSO GetConditionPlugin()
    {
        return GetAllPlugins().OfType<SceneConditionPluginSO>().FirstOrDefault();
    }

    public string GetConditionSummary()
    {
        var condition = GetConditionPlugin();
        if (condition == null) return "[조건 없음]";
        return condition.GetDebugName();
    }
}