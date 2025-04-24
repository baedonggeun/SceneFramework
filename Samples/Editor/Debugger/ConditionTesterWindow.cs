#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ConditionTesterWindow : EditorWindow
{
    private Object selectedTarget;
    private string presetResult;
    private bool conditionResult;
    private string debugInfo;
    private GUIStyle resultStyle;

    [MenuItem("Tools/Hybrid Scene Framework/Condition Tester")]
    public static void Open()
    {
        GetWindow<ConditionTesterWindow>("Condition Tester");
    }

    private void OnEnable()
    {
        resultStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
    }

    private void OnGUI()
    {
        GUILayout.Label("테스트 대상 선택", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        selectedTarget = EditorGUILayout.ObjectField("Condition or Preset", selectedTarget, typeof(Object), false);
        if (EditorGUI.EndChangeCheck())
        {
            EvaluateTarget();
        }

        if (selectedTarget != null)
        {
            GUILayout.Space(10);
            resultStyle.normal.textColor = conditionResult ? Color.green : Color.red;
            GUILayout.Label(conditionResult ? "조건 평가 결과: TRUE" : "조건 평가 결과: FALSE", resultStyle);

            GUILayout.Space(5);
            if (GUILayout.Button("다시 평가"))
            {
                EvaluateTarget();
            }

            GUILayout.Space(5);
            EditorGUILayout.HelpBox(debugInfo, MessageType.Info);
        }
    }

    private void EvaluateTarget()
    {
        if (selectedTarget is SceneTransitionConditionBase condition)
        {
            conditionResult = condition.Evaluate();
            debugInfo = $"조건 설명: {condition.GetDebugName()}";
        }
        else if (selectedTarget is ScenePresetSO preset)
        {
            var plugin = preset.GetConditionPlugin();
            if (plugin == null)
            {
                conditionResult = true;
                debugInfo = $"[ScenePresetSO] 조건 플러그인이 없습니다. (조건 없음)";
            }
            else
            {
                conditionResult = plugin.IsAllConditionsMet();
                debugInfo = $"[ScenePresetSO] 조건 요약: {plugin.GetDebugName()}";
            }
        }
        else
        {
            conditionResult = false;
            debugInfo = "지원하지 않는 타입입니다. (ConditionSO 또는 PresetSO)";
        }

        Debug.Log($"[ConditionTester] 결과: {conditionResult}, 설명: {debugInfo}");
    }
}
#endif