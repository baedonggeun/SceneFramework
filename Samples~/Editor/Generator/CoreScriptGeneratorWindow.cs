#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class CoreScriptGeneratorWindow : EditorWindow
{
    private enum GeneratorType { Service, KeySystem, AddressableSO }
    private GeneratorType selected = GeneratorType.Service;

    private string inputName = "MyClass";
    private string folderPath = "Assets";
    private Vector2 previewScroll;
    private Dictionary<string, string> previewFiles = new();

    private GeneratorType previousSelected;
    private string previousInputName;

    [MenuItem("Tools/Hybrid Scene Framework/Core Script Generator")]
    public static void ShowWindow()
    {
        GetWindow<CoreScriptGeneratorWindow>("Core Script Generator");
    }

    private void OnEnable()
    {
        GeneratePreview(); // 최초 1회 호출
        previousSelected = selected;
        previousInputName = inputName;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        // 왼쪽 영역 - 생성기 선택 및 입력
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        GUILayout.Label("Script Type", EditorStyles.boldLabel);

        if (GUILayout.Toggle(selected == GeneratorType.Service, "Service", "Button"))
            selected = GeneratorType.Service;
        if (GUILayout.Toggle(selected == GeneratorType.KeySystem, "Key System", "Button"))
            selected = GeneratorType.KeySystem;
        if (GUILayout.Toggle(selected == GeneratorType.AddressableSO, "Addressable SO", "Button"))
            selected = GeneratorType.AddressableSO;

        GUILayout.Space(20);
        GUILayout.Label("Class / Key Name", EditorStyles.boldLabel);
        inputName = EditorGUILayout.TextField(inputName);

        GUILayout.Label("Folder", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        folderPath = EditorGUILayout.TextField(folderPath);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
            if (!string.IsNullOrEmpty(path)) folderPath = path;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        if (GUILayout.Button("Generate", GUILayout.Height(30)))
        {
            GenerateScript();
        }

        EditorGUILayout.EndVertical();

        // 오른쪽 영역 - 프리뷰
        EditorGUILayout.BeginVertical();

        GUILayout.Label("Preview", EditorStyles.boldLabel);

        previewScroll = EditorGUILayout.BeginScrollView(previewScroll);
        if (previewFiles.Count == 0)
        {
            EditorGUILayout.HelpBox("미리볼 코드가 없습니다.", MessageType.Info);
        }
        else
        {
            foreach (var file in previewFiles)
            {
                EditorGUILayout.LabelField($"--- {file.Key} ---", EditorStyles.boldLabel);

                GUI.enabled = false;
                EditorGUILayout.TextArea(file.Value ?? "", GUILayout.ExpandHeight(false));
                GUI.enabled = true;

                GUILayout.Space(10);
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        // 변경 감지 후 Preview 갱신
        if (selected != previousSelected || inputName != previousInputName)
        {
            previousSelected = selected;
            previousInputName = inputName;
            GeneratePreview();
        }
    }

    private void GenerateScript()
    {
        Directory.CreateDirectory(folderPath); // 경로 보장

        switch (selected)
        {
            case GeneratorType.Service:
                CoreScriptGeneratorUtility.GenerateServiceScript(inputName, folderPath);
                break;
            case GeneratorType.KeySystem:
                CoreScriptGeneratorUtility.GenerateKeySystemScript(inputName, folderPath);
                break;
            case GeneratorType.AddressableSO:
                CoreScriptGeneratorUtility.GenerateAddressableSO(inputName, folderPath);
                break;
        }

        AssetDatabase.Refresh();
    }

    private void GeneratePreview()
    {
        previewFiles.Clear();

        if (string.IsNullOrWhiteSpace(inputName))
            return;

        switch (selected)
        {
            case GeneratorType.Service:
                previewFiles["__INFO__.txt"] =
                    "// Service 클래스 및 인터페이스 생성됨\n" +
                    "// MonoSingleton 기반, DI 및 초기화 지원";

                string interfaceName = $"I{inputName}";
                previewFiles[$"{inputName}.cs"] = ServiceClassPreview(inputName);
                previewFiles[$"{interfaceName}.cs"] = ServiceInterfacePreview(interfaceName);
                break;

            case GeneratorType.KeySystem:
                previewFiles["__INFO__.txt"] =
                    "// Key enum + RegistrySO + Static accessor 생성됨\n" +
                    "// Registry 인스턴스는 Resources/KeysSO 경로에 생성되어야 합니다.";

                previewFiles[$"{inputName}KeyRegistrySO.cs"] = KeyRegistryPreview(inputName);
                previewFiles[$"{inputName}Keys.cs"] = KeyAccessorPreview(inputName);
                break;

            case GeneratorType.AddressableSO:
                previewFiles["__INFO__.txt"] =
                    $"// Addressable용 ScriptableObject 생성됨\n" +
                    $"// 메뉴: Game/{inputName}";

                previewFiles[$"{inputName}.cs"] = AddressableSOPreview(inputName);
                break;
        }
    }

    private string ServiceClassPreview(string className)
    {
        return CoreScriptGeneratorUtility.ServiceScriptString(className);
    }

    private string ServiceInterfacePreview(string interfaceName)
    {
        return CoreScriptGeneratorUtility.IServiceScriptString(interfaceName);
    }

    private string KeyRegistryPreview(string name)
    {
        string enumName = $"{name}Key";
        string registrySOName = $"{name}KeyRegistrySO";
        string accessorName = $"{name}Keys";

        return CoreScriptGeneratorUtility.KeyRegistrySOScriptString(enumName, registrySOName, accessorName);
    }

    private string KeyAccessorPreview(string name)
    {
        string enumName = $"{name}Key";
        string registrySOName = $"{name}KeyRegistrySO";
        string accessorName = $"{name}Keys";

        return CoreScriptGeneratorUtility.KeyAccessorScriptString(enumName, registrySOName, accessorName);
    }

    private string AddressableSOPreview(string className)
    {
        return CoreScriptGeneratorUtility.AddressableSOScriptString(className);
    }
}
#endif