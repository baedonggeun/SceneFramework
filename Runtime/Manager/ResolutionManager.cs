using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public enum RKey
{
    ResolutionIndex,
    Fullscreen,
    CustomWidth,
    CustomHeight,
}

public class ResolutionManager : MonoSingleton<ResolutionManager>, IResolutionManager, IInitializable, IInjectable
{
    public int Priority => 0;
    public bool AutoInitialize => true;
    public Type[] GetDependencies() => Array.Empty<Type>();

    public struct supportedResolution
    {
        public int width;
        public int height;

        public supportedResolution(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    private supportedResolution defaultResolution;

    public supportedResolution[] resolutions = new supportedResolution[]
    {
        new supportedResolution(1280, 720),
        new supportedResolution(1366, 768),
        new supportedResolution(1600, 900),
        new supportedResolution(1920, 1080),
    };

    public async UniTask InitializeAsync()
    {
        await UniTask.Delay(100); // Simulated Init
    }

    protected override void Awake()
    {
        base.Awake();

        defaultResolution = resolutions[3];
    }

    public void SetWindowResolution(int index)
    {
        if (index < 0 || index >= resolutions.Length)
        {
            Screen.SetResolution((int)defaultResolution.width, (int)defaultResolution.height, false);
            return;
        }

        Screen.SetResolution(resolutions[index].width, resolutions[index].height, false);

        SaveWindowResolution(index);
    }

    public void SetFullScreenResolution(bool isFullScreen)
    {
        Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        SaveFullScreenResolution(isFullScreen);
    }

    private void SaveWindowResolution(int index)
    {
        PlayerPrefs.SetInt(RKey.ResolutionIndex.ToString(), index);

        PlayerPrefs.DeleteKey(RKey.CustomWidth.ToString());
        PlayerPrefs.DeleteKey(RKey.CustomHeight.ToString());

        PlayerPrefs.Save();
    }

    private void SaveFullScreenResolution(bool isFullScreen)
    {
        PlayerPrefs.SetInt(RKey.Fullscreen.ToString(), isFullScreen ? 1 : 0);

        PlayerPrefs.Save();
    }

    public bool LoadSavedResolution()
    {
        if (!Screen.fullScreen && PlayerPrefs.HasKey(RKey.ResolutionIndex.ToString()))
        {
            int savedIndex = PlayerPrefs.GetInt(RKey.ResolutionIndex.ToString());

            SetWindowResolution(savedIndex);

            return true;
        }

        return false;
    }
}