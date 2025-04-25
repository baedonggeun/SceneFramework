using System;
using UnityEngine;

public class UISliderVolumeControl : UISliderBase, IInjectable
{
    [Inject] private IAudioManager _audioManager;
    public Type[] GetDependencies() => new Type[] { typeof(IAudioManager) };
    [SerializeField] private SourceName source;

    protected override void OnValueChanged(float value)
    {
        _audioManager.SetVolume(source, value);

        PlayerPrefs.SetFloat("previous_" + source, PlayerPrefs.GetFloat(source.ToString()));
        PlayerPrefs.SetFloat(source.ToString(), value);
        PlayerPrefs.Save();
    }
}