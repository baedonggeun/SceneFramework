using System;
using UnityEngine;

public class UIButtonSetResolution : UIButtonBase, IInjectable
{
    [Inject] private IResolutionManager _resolutionManager;
    public Type[] GetDependencies() => new Type[] { typeof(IResolutionManager) };

    [SerializeField] private int resolutionIndex;

    protected override void OnClick()
    {
        _resolutionManager.SetWindowResolution(resolutionIndex);
    }
}