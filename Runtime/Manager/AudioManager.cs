using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using System.Linq;

public enum SourceName
{
    Master, BGM, SFX
}

public enum BGMName
{
    BGM_Main
}

public enum SFXName
{
    SFX_Break
}

public class AudioManager : MonoSingleton<AudioManager>, IAudioManager, IInitializable, IInjectable
{
    public int Priority => 2;
    public bool AutoInitialize => true;
    public Type[] GetDependencies() => Array.Empty<Type>();

    private AudioMixer masterMixer;

    private Dictionary<SourceName, AudioSource> audioSources;

    private Dictionary<BGMName, AudioClip> bgmClip;
    private Dictionary<SFXName, AudioClip> sfxClip;

    public async UniTask InitializeAsync()
    {
        await UniTask.Delay(100); // Simulated Init
    }

    protected override void Awake()
    {
        base.Awake();

        masterMixer = Resources.Load<AudioMixer>("Audio/Master");

        InitOrAttachSources();
        InitAudioClip();
        InitPlayerPrefs();
    }

    private void Start()
    {
        SetAudioClip(SourceName.BGM, GetBGMClip(BGMName.BGM_Main), loop: true, playOnAwake: false);
        Play(SourceName.BGM);
    }

    private void InitOrAttachSources()
    {
        audioSources = new Dictionary<SourceName, AudioSource>();

        List<AudioSource> foundSources = GetComponentsInChildren<AudioSource>().ToList();

        if (foundSources.Count == 0)
        {
            // 자동 생성
            foreach (SourceName name in System.Enum.GetValues(typeof(SourceName)))
            {
                GameObject go = new GameObject(name.ToString());
                go.transform.SetParent(transform);
                AudioSource src = go.AddComponent<AudioSource>();
                foundSources.Add(src); // 추가된 걸 리스트에 포함시킴
            }
        }

        // 초기화
        foreach (AudioSource source in foundSources)
        {
            string cleanedName = RemoveAllExceptSourceName(source.name);

            if (System.Enum.TryParse(cleanedName, out SourceName sourceName))
            {
                var groups = masterMixer.FindMatchingGroups($"Master/{sourceName}");
                if (groups.Length == 0)
                    groups = masterMixer.FindMatchingGroups(sourceName.ToString());

                if (groups.Length > 0)
                    source.outputAudioMixerGroup = groups[0];

                audioSources.Add(sourceName, source);
            }
        }
    }

    private void InitAudioClip()
    {
        bgmClip = new Dictionary<BGMName, AudioClip>();
        sfxClip = new Dictionary<SFXName, AudioClip>();

        foreach (var clip in Resources.LoadAll<AudioClip>("Audio/BGM/"))
        {
            if (System.Enum.TryParse(clip.name, out BGMName bgm))
            {
                bgmClip.Add(bgm, clip);
            }
        }

        foreach (var clip in Resources.LoadAll<AudioClip>("Audio/SFX/"))
        {
            if (System.Enum.TryParse(clip.name, out SFXName sfx))
            {
                sfxClip.Add(sfx, clip);
            }
        }
    }

    private void InitPlayerPrefs()
    {
        string key;
        string previousKey;

        foreach (SourceName source in Enum.GetValues(typeof(SourceName)))
        {
            key = source.ToString();
            previousKey = "previous_" + key;

            if (!PlayerPrefs.HasKey(key))
                PlayerPrefs.SetFloat(key, 1f);

            if (!PlayerPrefs.HasKey(previousKey))
                PlayerPrefs.SetFloat(previousKey, 1f);
        }

        PlayerPrefs.Save();
    }

    public string RemoveAllExceptSourceName(string input)
    {
        foreach (SourceName source in Enum.GetValues(typeof(SourceName)))
        {
            if (input.Contains(source.ToString()))
            {
                return source.ToString();
            }
        }
        return string.Empty;
    }

    public void SetAudioClip(SourceName source, AudioClip clip, bool loop = false, bool playOnAwake = false)
    {
        if (!audioSources.TryGetValue(source, out var audioSource) || audioSource == null)
        {
            Debug.LogWarning($"[AudioManager] AudioSource {source}가 존재하지 않습니다.");
            return;
        }

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.playOnAwake = playOnAwake;
    }

    public void Play(SourceName source)
    {
        if (audioSources.TryGetValue(source, out var audioSource) && audioSource.clip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"[AudioManager] AudioSource {source}가 비어 있거나 클립이 없습니다.");
        }
    }

    public void Stop(SourceName source)
    {
        if (audioSources.TryGetValue(source, out AudioSource sourceAudio))
            sourceAudio.Stop();
    }

    public AudioClip GetBGMClip(BGMName name) => bgmClip.TryGetValue(name, out var clip) ? clip : null;

    public AudioClip GetSFXClip(SFXName name) => sfxClip.TryGetValue(name, out var clip) ? clip : null;

    public void PlayBGM(BGMName name, bool loop)
    {
        var clip = GetBGMClip(name);
        if (clip != null)
        {
            SetAudioClip(SourceName.BGM, clip, loop, playOnAwake: false);
            Play(SourceName.BGM);
        }
    }

    public void PlaySFX(SFXName name, bool loop)
    {
        var clip = GetSFXClip(name);
        if (clip != null)
        {
            SetAudioClip(SourceName.SFX, clip, loop, playOnAwake: false);
            Play(SourceName.SFX);
        }
    }

    public void MasterMixerController(SourceName source, float volume)
    {
        string key = source.ToString();
        string previousKey = "previous_" + key;

        if (masterMixer == null)
        {
            Debug.LogError("[AudioManager] masterMixer가 null입니다!");
            return;
        }

        volume = Mathf.Max(volume, 0.0001f);
        masterMixer.SetFloat(source.ToString(), Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat(previousKey, PlayerPrefs.GetFloat(key));
        PlayerPrefs.SetFloat(key, volume);
        PlayerPrefs.Save();
    }

    public void LoasSavedVolume()
    {
        foreach (SourceName source in Enum.GetValues(typeof(SourceName)))
        {
            if (PlayerPrefs.HasKey(source.ToString()))
            {
                float savedVolume = PlayerPrefs.GetFloat(source.ToString());
                MasterMixerController(source, savedVolume);
            }
        }
    }

    public float SetVolume(SourceName source, float volume)
    {
        string key = source.ToString();
        string previousKey = "previous_" + key;

        PlayerPrefs.SetFloat(previousKey, PlayerPrefs.GetFloat(key));
        PlayerPrefs.SetFloat(key, volume);
        PlayerPrefs.Save();

        if (audioSources.TryGetValue(source, out AudioSource audioSource))
        {
            audioSource.volume = volume;
        }

        return volume;
    }
}