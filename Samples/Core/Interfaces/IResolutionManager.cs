using Cysharp.Threading.Tasks;
using UnityEngine;
using static ResolutionManager;

public interface IResolutionManager
{
    void SetWindowResolution(int index);
    void SetFullScreenResolution(bool isFullScreen);
    bool LoadSavedResolution();
}
