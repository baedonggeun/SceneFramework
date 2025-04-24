using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "HybridSceneFramework/KeyRegistry/PrefabKeyRegistry")]
public class PrefabKeyRegistrySO : ScriptableObject
{
    [System.Serializable]
    public class PrefabKeyEntry
    {
        public PrefabKey key;
        public string addressableKey;
    }

    [SerializeField] private List<PrefabKeyEntry> entries;
    private Dictionary<PrefabKey, string> lookup;

    public string GetKey(PrefabKey key)
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