using UnityEngine;

public static class PrefabKeys
{
    public static PrefabKeyRegistrySO Registry;

    public static void LoadRegistry()
    {
        if (Registry != null) return;

        Registry = Resources.Load<PrefabKeyRegistrySO>("KeysSO/SOKeyRegistry");
        if (Registry == null)
            Debug.LogError("[PrefabKeys] Registry가 Resources 폴더에 없습니다.");
    }

    public static string Get(PrefabKey key)
    {
        LoadRegistry();
        return Registry?.GetKey(key);
    }
}