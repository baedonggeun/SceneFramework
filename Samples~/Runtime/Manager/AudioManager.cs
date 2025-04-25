using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : MonoSingleton<AudioManager>, IAudioManager, IInitializable, IInjectable
{
    public int Priority => 2;
    public bool AutoInitialize => true;
    public Type[] GetDependencies() => Array.Empty<Type>();

    private Dictionary<AudioMixerKey, AudioMixer> audioMixer = new();
    private readonly Dictionary<string, AudioMixer> exposedParams = new();

    private Dictionary<SourceName, AudioSource> audioSources;

    private Dictionary<BGMName, AudioClip> bgmClip;
    private Dictionary<SFXName, AudioClip> sfxClip;

    public async UniTask InitializeAsync()
    {
        await UniTask.Delay(100); // Simulated Init
    }

    protected override async void Awake()
    {
        base.Awake();

        await LoadAllMixersAsync();
        CacheExposedParameters();

        InitOrAttachSources();
        InitAudioClip();
        InitPlayerPrefs();

        LoadSavedVolume();
    }

    private async UniTask LoadAllMixersAsync()
    {
        AudioMixerKey[] keys = (AudioMixerKey[])Enum.GetValues(typeof(AudioMixerKey));
        foreach (var key in keys)
        {
            string path = AudioKeys.Get(key);
            if (string.IsNullOrEmpty(path)) continue;

            var handle = Addressables.LoadAssetAsync<AudioMixer>(path);
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                audioMixer[key] = handle.Result;
            }
            else
            {
                Debug.LogError($"[AudioManager] Failed to load mixer: {key} ({path})");
            }
        }
    }

    private void CacheExposedParameters()
    {
        foreach (var mixerPair in audioMixer)
        {
            var mixer = mixerPair.Value;

            foreach (SourceName source in Enum.GetValues(typeof(SourceName)))
            {
                string param = source.ToString();

                // 이 SetFloat은 실패 여부를 알려주지 않기 때문에,
                // 뒤에서 GetFloat으로 진짜 존재하는 파라미터인지 확인
                mixer.SetFloat(param, 0f); // 먼저 설정 시도 (없으면 무시됨)

                float testValue;
                if (mixer.GetFloat(param, out testValue)) // 성공 시 → 실제로 존재하는 파라미터
                {
                    exposedParams[param] = mixer;
                }
            }
        }
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

            if (Enum.TryParse(cleanedName, out SourceName sourceName))
            {
                AudioMixerGroup foundGroup = null;

                foreach (var kvp in audioMixer)
                {
                    var groups = kvp.Value.FindMatchingGroups(sourceName.ToString());
                    if (groups.Length > 0)
                    {
                        foundGroup = groups[0];
                        break;
                    }
                }

                if (foundGroup != null)
                {
                    source.outputAudioMixerGroup = foundGroup;
                    audioSources[sourceName] = source;
                }
                else
                {
                    Debug.LogWarning($"[AudioManager] AudioMixerGroup '{sourceName}'를 찾을 수 없습니다.");
                }
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
        if (!audioSources.TryGetValue(source, out var audioSource))
        {
            Debug.LogWarning($"[AudioManager] AudioSource '{source}' 없음.");
            return;
        }

        if (audioSource.clip == null)
        {
            Debug.LogWarning($"[AudioManager] AudioSource '{source}'에 클립이 지정되지 않았습니다.");
            return;
        }

        audioSource.Play();
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

    private void SetExposedVolume(SourceName source, float volume)
    {
        string param = source.ToString();
        string previousKey = "previous_" + param;

        volume = Mathf.Max(volume, 0.0001f);
        float dB = Mathf.Log10(volume) * 20;

        if (exposedParams.TryGetValue(param, out var mixer))
        {
            mixer.SetFloat(param, dB);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] '{param}' 파라미터는 어떤 AudioMixer에도 노출되어 있지 않습니다.");
        }

        PlayerPrefs.SetFloat(previousKey, PlayerPrefs.GetFloat(param));
        PlayerPrefs.SetFloat(param, volume);
        PlayerPrefs.Save();
    }

    public void LoadSavedVolume()
    {
        foreach (SourceName source in Enum.GetValues(typeof(SourceName)))
        {
            if (PlayerPrefs.HasKey(source.ToString()))
            {
                float savedVolume = PlayerPrefs.GetFloat(source.ToString());
                SetExposedVolume(source, savedVolume);
            }
        }
    }

    public float SetVolume(SourceName source, float volume)
    {
        // 1. AudioMixer의 exposed parameter 볼륨 조절
        SetExposedVolume(source, volume);

        // 2. AudioSource 자체 볼륨 조절
        if (audioSources.TryGetValue(source, out AudioSource audioSource))
        {
            audioSource.volume = volume;
        }

        return volume;
    }
}