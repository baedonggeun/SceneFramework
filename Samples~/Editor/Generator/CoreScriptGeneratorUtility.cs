#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class CoreScriptGeneratorUtility
{
    public static void GenerateServiceScript(string className, string folderPath)
    {
        string interfaceName = $"I{className}";

        string classContent = ServiceScriptString(className);

        string interfaceContent = IServiceScriptString(interfaceName);

        File.WriteAllText(Path.Combine(folderPath, $"{className}.cs"), classContent);
        File.WriteAllText(Path.Combine(folderPath, $"{interfaceName}.cs"), interfaceContent);
    }

    public static string ServiceScriptString(string className)
    {
        string interfaceName = $"I{className}";

        return $@"using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class {className} : MonoSingleton<{className}>, {interfaceName}, IInitializable, IInjectable
{{
    public int Priority => 0;
    public bool AutoInitialize => true;

    public Type[] GetDependencies() => Array.Empty<Type>();

    public async UniTask InitializeAsync()
    {{
        await UniTask.Yield(); // Init logic here
    }}
}}";
    }

    public static string IServiceScriptString(string interfaceName)
    {
        return $@"public interface {interfaceName}
{{
    // Define public API here
}}";
    }

    public static void GenerateKeySystemScript(string keySystemName, string folderPath)
    {
        string keysSOFolder = "Assets/Resources/KeysSO";
        if (!Directory.Exists(keysSOFolder))
            Directory.CreateDirectory(keysSOFolder);

        string enumName = $"{keySystemName}Key";
        string registrySOName = $"{keySystemName}KeyRegistrySO";
        string accessorName = $"{keySystemName}Keys";

        string registryContent = KeyRegistrySOScriptString(enumName, registrySOName, accessorName);

        string accessorContent = KeyAccessorScriptString(enumName, registrySOName, accessorName);

        File.WriteAllText(Path.Combine(folderPath, $"{registrySOName}.cs"), registryContent);
        File.WriteAllText(Path.Combine(folderPath, $"{accessorName}.cs"), accessorContent);
    }

    public static string KeyRegistrySOScriptString(string enumName, string registrySOName, string accessorName)
    {
        return $@"using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = ""HybridSceneFramework/KeyRegistry/{registrySOName}"")]
public class {registrySOName} : ScriptableObject
{{
    [System.Serializable]
    public class {enumName}Entry
    {{
        public {enumName} key;
        public string addressableKey;
    }}

    [Header(""Enum 타입 key <-> addressable key 문자열 매핑"")]
    [SerializeField] private List<{enumName}Entry> entries;
    private Dictionary<{enumName}, string> lookup;

    public string GetKey({enumName} key)
    {{
        if (lookup == null)
        {{
            lookup = new();
            foreach (var e in entries)
                lookup[e.key] = e.addressableKey;
        }}
        return lookup.TryGetValue(key, out var value) ? value : null;
    }}
}}";
    }

    public static string KeyAccessorScriptString(string enumName, string registrySOName, string accessorName)
    {
        return $@"using UnityEngine;

public static class {accessorName}
{{
    public static {registrySOName} Registry;

    public static void LoadRegistry()
    {{
        if (Registry != null) return;

        Registry = Resources.Load<{registrySOName}>(""KeysSO/{registrySOName}"");
        if (Registry == null)
            Debug.LogError(""[{accessorName}] Registry가 Resources 폴더에 없습니다."");
    }}

    public static string Get({enumName} key)
    {{
        LoadRegistry();
        return Registry?.GetKey(key);
    }}
}}";
    }

    public static void GenerateAddressableSO(string className, string folderPath)
    {
        string content = AddressableSOScriptString(className);

        File.WriteAllText(Path.Combine(folderPath, $"{className}.cs"), content);
    }

    public static string AddressableSOScriptString(string className)
    {
        return $@"using UnityEngine;

[CreateAssetMenu(fileName = ""{className}"", menuName = ""Game/{className}"")]
public class {className} : ScriptableObject
{{
}}";
    }
}
#endif