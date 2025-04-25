using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class SceneServiceManager : MonoSingleton<SceneServiceManager>, ISceneServiceManager, IInitializable, IInjectable
{
    public int Priority => 0;
    public bool AutoInitialize => true;
    public Type[] GetDependencies() => Array.Empty<Type>();

    private readonly Dictionary<string, ScenePresetSO> scenePresets = new();
    private ScenePresetSO currentPreset;

    public static InjectRegistry CurrentRegistry { get; private set; }

    public static event Action OnPreloadComplete;

    protected override void Awake()
    {
        base.Awake();
        LoadCurrentScenePreset().Forget();
    }

    public async UniTask InitializeAsync()
    {
        await UniTask.Yield();
    }

    public async UniTask LoadCurrentScenePreset()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        currentPreset = await LoadPresetForScene(currentSceneName);

        if (currentPreset == null)
        {
            Debug.LogWarning($"[SceneServiceManager] Preset not found for scene: {currentSceneName}");
            return;
        }

        await LoadScenePreset(currentPreset);
    }

    private async UniTask<ScenePresetSO> LoadPresetForScene(string sceneName)
    {
        AsyncOperationHandle<IList<ScenePresetSO>> handle =
            Addressables.LoadAssetsAsync<ScenePresetSO>(sceneName, _ => { });

        await handle.ToUniTask();

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("[SceneServiceManager] Failed to load presets");
            return null;
        }

        foreach (var preset in handle.Result)
        {
            if (!scenePresets.ContainsKey(preset.SceneName))
            {
                scenePresets[preset.SceneName] = preset;
            }
        }

        scenePresets.TryGetValue(sceneName, out var result);
        return result;
    }

    public async UniTask LoadServicesForScene(string sceneName)
    {
        if (!scenePresets.TryGetValue(sceneName, out var preset))
        {
            Debug.LogWarning($"[SceneServiceManager] No preset for scene: {sceneName}");
            return;
        }

        await LoadScenePreset(preset);
    }

    public async UniTask LoadScenePreset(ScenePresetSO newPreset)
    {
        Debug.Log($"[SceneServiceManager] Loading preset: {newPreset.SceneName}");

        //1. 이전 preset 언로드
        if (currentPreset != null &&
            currentPreset.AutoUnloadOnSceneChange &&
            currentPreset.SceneType != SceneKey.Global)
        {
            await ScenePluginExecutor.ExecuteOnUnload(currentPreset);
        }

        //2. 새로운 preset 설정
        currentPreset = newPreset;

        //3. InjectRegistry 재생성 및 공유 저장소 업데이트
        CurrentRegistry = InjectRegistry.BuildFromScenePreset(currentPreset);

        //4. 새로운 preset 로드 및 플러그인 로드 시 DI 컨테이너 공유
        await ScenePluginExecutor.ExecuteOnLoad(currentPreset, CurrentRegistry);

        OnPreloadComplete?.Invoke();
    }
}