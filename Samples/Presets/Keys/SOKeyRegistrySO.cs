using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "HybridSceneFramework/KeyRegistry/SOKeyRegistry")]
public class SOKeyRegistrySO : ScriptableObject
{
    [System.Serializable]
    public class SOKeyEntry
    {
        public SOKey key;
        public string addressableKey;
    }

    [SerializeField] private List<SOKeyEntry> entries;
    private Dictionary<SOKey, string> lookup;

    public string GetKey(SOKey key)
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