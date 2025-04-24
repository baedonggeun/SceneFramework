using System;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonToggleFullScreen : UIButtonBase, IInjectable
{
    [Inject] private IResolutionManager _resolutionManager;
    public Type[] GetDependencies() => new Type[] { typeof(IResolutionManager) };

    [SerializeField] private bool isFullScreen;
    [SerializeField] private List<GameObject> resolutionButtonsToToggle;

    protected override void OnClick()
    {
        _resolutionManager.SetFullScreenResolution(isFullScreen);

        foreach (var button in resolutionButtonsToToggle)
        {
            button.SetActive(!isFullScreen);
        }
    }
}