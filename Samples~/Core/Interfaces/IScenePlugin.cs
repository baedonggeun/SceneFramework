using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IScenePlugin
{
    // 메타 정보
    string PluginName { get; }
    int Priority { get; }
    bool IsRequired { get; }

    // 분류 정보
    SceneKey TargetSceneType { get; }
    List<string> Labels { get; }

    // 실행 로직
    UniTask OnLoad(ScenePresetSO context, Dictionary<Type, object> instanceMap);
    UniTask OnLoad(ScenePresetSO context);
    UniTask OnUnload(ScenePresetSO context);
}