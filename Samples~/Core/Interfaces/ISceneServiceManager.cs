using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISceneServiceManager
{
    ScenePresetSO CurrentPreset { get; }

    UniTask LoadCurrentScenePreset();
    UniTask LoadServicesForScene(string sceneName);
    UniTask LoadScenePreset(ScenePresetSO newPreset);
}