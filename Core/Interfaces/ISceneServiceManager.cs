using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISceneServiceManager
{
    UniTask LoadCurrentScenePreset();
    UniTask LoadServicesForScene(string sceneName);
    UniTask LoadScenePreset(ScenePresetSO newPreset);
}