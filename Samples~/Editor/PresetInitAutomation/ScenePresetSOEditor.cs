#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScenePresetSO))]
public class ScenePresetSOEditor : Editor
{
    private SerializedProperty pluginsProp;

    private void OnEnable()
    {
        pluginsProp = serializedObject.FindProperty("plugins");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoUnloadOnSceneChange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("subPresets"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("plugins"));

        //EditorGUILayout.Space(10);
        //EditorGUILayout.LabelField("플러그인 리스트", EditorStyles.boldLabel);

        //for (int i = 0; i < pluginsProp.arraySize; i++)
        //{
        //    var element = pluginsProp.GetArrayElementAtIndex(i);
        //    EditorGUILayout.BeginHorizontal();

        //    EditorGUILayout.PropertyField(element, GUIContent.none);

        //    if (GUILayout.Button("↑", GUILayout.Width(25)) && i > 0)
        //        pluginsProp.MoveArrayElement(i, i - 1);

        //    if (GUILayout.Button("↓", GUILayout.Width(25)) && i < pluginsProp.arraySize - 1)
        //        pluginsProp.MoveArrayElement(i, i + 1);

        //    if (GUILayout.Button("✕", GUILayout.Width(25)))
        //        pluginsProp.DeleteArrayElementAtIndex(i);

        //    EditorGUILayout.EndHorizontal();
        //}

        //EditorGUILayout.Space();

        //if (GUILayout.Button("플러그인 추가"))
        //{
        //    pluginsProp.InsertArrayElementAtIndex(pluginsProp.arraySize);
        //}

        serializedObject.ApplyModifiedProperties();
    }
}
#endif