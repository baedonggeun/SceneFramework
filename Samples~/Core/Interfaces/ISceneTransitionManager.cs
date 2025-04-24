using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISceneTransitionManager
{
    UniTask<bool> TryTransition(string targetScene, bool additive = false, bool unloadCurrent = true);
}
