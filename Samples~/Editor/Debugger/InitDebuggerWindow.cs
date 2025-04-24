#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

public class InitDebuggerWindow : EditorWindow
{
    private Vector2 scroll;
    private bool showSuccess = true;
    private bool showFailed = true;
    private bool groupByInitGroup = true;

    [MenuItem("Tools/Hybrid Scene Framework/Debug/Open InitDebuggerWindow")]
    public static void OpenWindow()
    {
        GetWindow<InitDebuggerWindow>("Init Debugger");
    }

    private void OnGUI()
    {
        GUILayout.Label("Init Debugger Viewer", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        showSuccess = EditorGUILayout.Toggle("Show Success", showSuccess);
        showFailed = EditorGUILayout.Toggle("Show Failed", showFailed);
        GUILayout.EndHorizontal();

        groupByInitGroup = EditorGUILayout.Toggle("Group by InitGroup", groupByInitGroup);

        if (GUILayout.Button("Refresh"))
        {
            Repaint();
        }

        EditorGUILayout.Space();
        scroll = EditorGUILayout.BeginScrollView(scroll);

        var results = InitDebugger.GetResults();

        if (groupByInitGroup)
        {
            var groups = results.GroupBy(r => r.Group);
            foreach (var group in groups)
            {
                GUILayout.Label($"Group: {group.Key}", EditorStyles.boldLabel);

                foreach (var r in group)
                    DrawEntry(r);
            }
        }
        else
        {
            foreach (var r in results)
                DrawEntry(r);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawEntry(InitDebugger.InitResult r)
    {
        if ((r.Success && !showSuccess) || (!r.Success && !showFailed))
            return;

        Color prevColor = GUI.color;
        GUI.color = r.Success ? Color.green : new Color(1f, 0.4f, 0.4f);

        GUILayout.BeginHorizontal("box");
        GUILayout.Label(r.Name, GUILayout.Width(180));
        GUILayout.Label(r.Group, GUILayout.Width(120));
        GUILayout.Label($"{r.DurationMs:0.0} ms", GUILayout.Width(80));
        GUILayout.Label(r.Success ? "Success" : "Fail", GUILayout.Width(80));
        GUILayout.EndHorizontal();

        GUI.color = prevColor;
    }
}
#endif