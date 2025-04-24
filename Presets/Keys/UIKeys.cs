using UnityEngine;

public static class UIKeys
{
    public static UIKeyRegistrySO Registry;

    public static void LoadRegistry()
    {
        if (Registry != null) return;

        Registry = Resources.Load<UIKeyRegistrySO>("KeysSO/UIKeyRegistry");
        if (Registry == null)
            Debug.LogError("[UIKeys] Registry가 Resources 폴더에 없습니다.");
    }

    public static string Get(UIKey key)
    {
        LoadRegistry();
        return Registry?.GetKey(key);
    }
}