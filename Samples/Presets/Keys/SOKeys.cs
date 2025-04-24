using UnityEngine;

public static class SOKeys
{
    public static SOKeyRegistrySO Registry;

    public static void LoadRegistry()
    {
        if (Registry != null) return;

        Registry = Resources.Load<SOKeyRegistrySO>("KeysSO/SOKeyRegistry");
        if (Registry == null)
            Debug.LogError("[SOKeys] Registry가 Resources 폴더에 없습니다.");
    }

    public static string Get(SOKey key)
    {
        LoadRegistry();
        return Registry?.GetKey(key);
    }
}