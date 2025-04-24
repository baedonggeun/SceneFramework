using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

public enum SceneType
{
    Intro,
    Game,
    Result,
    Global
}

public abstract class ScenePluginSO : ScriptableObject, IScenePlugin
{
    [Header("플러그인 메타데이터")]
    [Tooltip("플러그인 이름 (중복되지 않도록 설정)")]
    [SerializeField] private string pluginName;

    [Header("플러그인 분류")]
    [SerializeField] private SceneType targetSceneType;

    [Tooltip("기능 태그 (UI, Analytics, DebugOnly 등)")]
    [SerializeField] private List<string> labels = new();

    [Header("실행 우선순위 (낮을수록 먼저 실행됨)")]
    [SerializeField] private int priority = 0;

    [Header("필수 등록 여부")]
    [SerializeField] private bool isRequired = true;

    public string PluginName => pluginName;
    public SceneType TargetSceneType => targetSceneType;
    public List<string> Labels => labels;
    public int Priority => priority;
    public bool IsRequired => isRequired;

    public virtual async UniTask OnLoad(ScenePresetSO context, Dictionary<Type, object> instanceMap) =>
        await UniTask.CompletedTask;

    public virtual async UniTask OnLoad(ScenePresetSO context) =>
        await OnLoad(context, null);


    public virtual UniTask OnUnload(ScenePresetSO context) => UniTask.CompletedTask;
}