using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionFSM
{
    private readonly List<ISceneTransitionState> _states;

    public SceneTransitionFSM(List<ISceneTransitionState> states)
    {
        _states = states;
    }

    public async UniTask Run(SceneTransitionContext context)
    {
        Debug.Log("[SceneTransitionFSM] 상태 실행 시작");

        for (int i = 0; i < _states.Count; i++)
        {
            var state = _states[i];

            try
            {
                Debug.Log($"[SceneTransitionFSM] ▶ 상태 진입: {state.GetType().Name}");
                await state.Enter(context);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SceneTransitionFSM] ❌ 상태 실패: {state.GetType().Name}\n{ex.Message}");
                break;
            }
        }

        Debug.Log("[SceneTransitionFSM] 상태 실행 종료");
    }
}