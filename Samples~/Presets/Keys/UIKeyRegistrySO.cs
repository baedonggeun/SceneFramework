using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "HybridSceneFramework/KeyRegistry/UIKeyRegistry")]
public class UIKeyRegistrySO : ScriptableObject
{
    [System.Serializable]
    public class UIKeyEntry
    {
        public UIKey key;
        public string addressableKey;
    }

    [Header("Enum 타입 key <-> addressable key 문자열 매핑")]
    [SerializeField] private List<UIKeyEntry> entries;
    private Dictionary<UIKey, string> lookup;

    public string GetKey(UIKey key)
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