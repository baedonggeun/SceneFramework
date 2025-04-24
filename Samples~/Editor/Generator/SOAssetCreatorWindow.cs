#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public class SOAssetCreatorWindow : EditorWindow
{
    private class Node
    {
        public string Name;
        public Dictionary<string, Node> Children = new();
        public List<Type> Types = new();
        public bool Expanded = true;
    }

    private Node root;
    private Vector2 leftScroll;
    private Vector2 rightScroll;
    private Type selectedType;

    private ScriptableObject previewInstance;
    private Editor previewEditor;

    private GUIStyle selectedStyle;
    private GUIStyle typeButtonStyle;

    [MenuItem("Tools/Hybrid Scene Framework/SO Asset Creator")]
    public static void ShowWindow()
    {
        GetWindow<SOAssetCreatorWindow>("SO Asset Creator");
    }

    private void OnEnable()
    {
        BuildTree();

        // 선택된 노드 하이라이트 스타일
        selectedStyle = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = Color.white, background = Texture2D.grayTexture }
        };

        // 클릭 가능한 타입 스타일 (노란색)
        typeButtonStyle = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = Color.yellow }
        };

        // 기본 프리뷰 하나 보여주기
        CreatePreview();
    }

    private void OnDisable()
    {
        ClearPreview();
    }

    private void BuildTree()
    {
        root = new Node { Name = "root" };

        var monoScripts = AssetDatabase.FindAssets("t:MonoScript")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
            .Where(script => script != null)
            .ToList();

        var types = monoScripts
            .Select(ms => ms.GetClass())
            .Where(type =>
                type != null &&
                typeof(ScriptableObject).IsAssignableFrom(type) &&
                type.GetCustomAttribute<CreateAssetMenuAttribute>() != null &&
                !type.IsAbstract)
            .Distinct();

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<CreateAssetMenuAttribute>();
            var parts = attr.menuName.Split('/');

            Node current = root;
            foreach (string part in parts.Take(parts.Length - 1))
            {
                if (!current.Children.ContainsKey(part))
                    current.Children[part] = new Node { Name = part };
                current = current.Children[part];
            }

            current.Types.Add(type);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        // 좌측 트리 영역
        EditorGUILayout.BeginVertical(GUILayout.Width(300));
        leftScroll = EditorGUILayout.BeginScrollView(leftScroll);
        DrawTreeNode(root, 0);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        // 우측 프리뷰 영역
        EditorGUILayout.BeginVertical();

        // 버튼 영역
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = selectedType != null;
        if (GUILayout.Button("Create SO Asset", GUILayout.Height(30)))
        {
            CreateSelectedAsset();
        }
        if (GUILayout.Button("Clear Preview", GUILayout.Width(120), GUILayout.Height(30)))
        {
            ClearPreview();
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        // 프리뷰 캔버스
        rightScroll = EditorGUILayout.BeginScrollView(rightScroll);
        DrawPreview();
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTreeNode(Node node, int indent)
    {
        foreach (var child in node.Children.Values)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indent * 16);
            child.Expanded = EditorGUILayout.Foldout(child.Expanded, child.Name, true);
            GUILayout.EndHorizontal();

            if (child.Expanded)
            {
                DrawTreeNode(child, indent + 1);
            }
        }

        foreach (var type in node.Types)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indent * 16);

            var style = type == selectedType ? selectedStyle : typeButtonStyle;
            if (GUILayout.Button(type.Name, style))
            {
                selectedType = type;
                CreatePreview();
            }

            GUILayout.EndHorizontal();
        }
    }

    private void CreateSelectedAsset()
    {
        if (selectedType == null) return;

        ScriptableObject instance = ScriptableObject.CreateInstance(selectedType);
        string defaultName = $"New{selectedType.Name}.asset";
        string path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", defaultName, "asset", $"Create a new {selectedType.Name} asset");

        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = instance;

            Debug.Log($"Created new SO asset: {path}");
        }
    }

    private void CreatePreview()
    {
        ClearPreview();

        if (selectedType == null) return;

        previewInstance = ScriptableObject.CreateInstance(selectedType);
        previewEditor = Editor.CreateEditor(previewInstance);
    }

    private void ClearPreview()
    {
        if (previewEditor != null)
        {
            DestroyImmediate(previewEditor);
            previewEditor = null;
        }

        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
        }
    }

    private void DrawPreview()
    {
        if (previewEditor != null)
        {
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            previewEditor.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUILayout.HelpBox("Select a ScriptableObject type from the list.", MessageType.Info);
        }
    }
}
#endif