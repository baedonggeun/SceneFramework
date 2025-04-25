using System;
using UnityEngine;

public class UIButtonMuteToggle : UIButtonBase, IInjectable
{
    [Inject] private IAudioManager _audioManager;
    public Type[] GetDependencies() => new Type[] { typeof(IAudioManager) };


    [SerializeField] private SourceName source;

    protected override void OnClick()
    {
        string key = source.ToString();
        string previousKey = "previous_" + key;

        float current = PlayerPrefs.GetFloat(key);
        float previous = PlayerPrefs.GetFloat(previousKey);

        if (current > 0.1f)
            _audioManager.SetVolume(source, 0f);
        else
            _audioManager.SetVolume(source, previous);
    }
}