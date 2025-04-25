using UnityEngine;

public static class AudioKeys
{
    public static AudioKeyRegistrySO Registry;

    public static void LoadRegistry()
    {
        if (Registry != null) return;

        Registry = Resources.Load<AudioKeyRegistrySO>("KeysSO/AudioKeyRegistrySO");
        if (Registry == null)
            Debug.LogError("[AudioKeys] Registry가 Resources 폴더에 없습니다.");
    }

    public static string Get(AudioMixerKey key)
    {
        LoadRegistry();
        return Registry?.GetKey(key);
    }
}