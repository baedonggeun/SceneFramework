using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IAudioManager
{
    UniTask InitializeAsync();
    void Play(SourceName source);
    void Stop(SourceName source);
    void SetAudioClip(SourceName source, AudioClip clip, bool loop = false, bool playOnAwake = false);
    void PlayBGM(BGMName name, bool loop);
    void PlaySFX(SFXName name, bool loop);
    void LoadSavedVolume();
    float SetVolume(SourceName source, float volume);
}