using System;
using UnityEngine;

public class UIButtonOpenByName : UIButtonBase, IInjectable
{
    [Inject] private IUIManager _uiManager;
    public Type[] GetDependencies() => new Type[] { typeof(IUIManager) };
    [SerializeField] private string popupName;

    protected override async void OnClick()
    {
        await _uiManager.ShowByNameAsync(popupName);
    }
}

