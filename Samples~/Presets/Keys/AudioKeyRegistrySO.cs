using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "HybridSceneFramework/KeyRegistry/AudioKeyRegistrySO")]
public class AudioKeyRegistrySO : ScriptableObject
{
    [System.Serializable]
    public class AudioKeyEntry
    {
        public AudioMixerKey key;
        public string addressableKey;
    }

    [SerializeField] private List<AudioKeyEntry> entries;
    private Dictionary<AudioMixerKey, string> lookup;

    public string GetKey(AudioMixerKey key)
    {
        if (lookup == null)
        {
            lookup = new();
            foreach (var e in entries)
                lookup[e.key] = e.addressableKey;
        }
        return lookup.TryGetValue(key, out var value) ? value : null;
    }
}