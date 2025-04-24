using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonSceneTransition : UIButtonBase, IInjectable
{
    [Inject] private ISceneTransitionManager _sceneTransitionManager;
    [Inject] private IUIManager _uiManager;
    public Type[] GetDependencies() => new Type[] { typeof(ISceneTransitionManager), typeof(IUIManager) };

    [SerializeField] private string presetKey;

    protected override async void OnClick()
    {
        bool result = await _sceneTransitionManager.TryTransition(presetKey);
        if (!result)
        {
            _uiManager?.ShowMessage("조건이 충족되지 않아 씬 전이 불가");
        }
    }
}
