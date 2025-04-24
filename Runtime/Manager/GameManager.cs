using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEditor;

public class GameManager : MonoSingleton<GameManager>, IGameManager, IInitializable, IInjectable
{
    public int Priority => 0;
    public bool AutoInitialize => true;

    public Type[] GetDependencies() => Array.Empty<Type>();

    public async UniTask InitializeAsync()
    {
        await UniTask.Delay(100); // Simulated Init
    }

}