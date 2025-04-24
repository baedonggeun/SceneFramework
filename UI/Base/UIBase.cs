using System;
using System.Collections.Generic;
using UnityEngine;

// 모든 UI 프리팹의 베이스 클래스입니다.
// 자동 캐싱 + 이름 기반 접근 혼용을 지원하며, 프리팹별 기본 캐시 타입을 오버라이딩 가능하게 합니다.
public abstract class UIBase : MonoBehaviour
{
    public Canvas canvas;

    private readonly Dictionary<Type, Dictionary<string, Component>> _typedCache = new();

    public virtual void Opened(params object[] param)
    {
        // 기본 캐시 타입 자동 로딩
        Type[] types = GetDefaultCacheTypes();
        if (types != null && types.Length > 0)
            CacheAllUI(types);
    }

    public virtual void Hide()
    {
        UIManager.Instance.Hide(gameObject.name);
    }

    public virtual bool IsGlobalUI() => false;

    // 이 UI에서 기본적으로 캐싱할 컴포넌트 타입 목록을 반환합니다.
    // 하위 클래스에서 오버라이드하여 설정하세요.
    protected virtual Type[] GetDefaultCacheTypes() => Array.Empty<Type>();

    protected virtual T GetUI<T>(string name) where T : Component
    {
        Type type = typeof(T);

        if (_typedCache.TryGetValue(type, out var typedDict))
        {
            if (typedDict.TryGetValue(name, out var cached))
                return cached as T;
        }

        Transform target = transform.Find(name);
        if (target == null)
        {
            Debug.LogWarning($"[UIBase] UI not found: {name}");
            return null;
        }

        T comp = target.GetComponent<T>();
        if (comp == null)
        {
            Debug.LogWarning($"[UIBase] Component <{type.Name}> not found on {name}");
            return null;
        }

        if (!_typedCache.ContainsKey(type))
            _typedCache[type] = new Dictionary<string, Component>();

        _typedCache[type][name] = comp;
        return comp;
    }

    protected void CacheAllUI(Type[] types)
    {
        foreach (var type in types)
        {
            if (!typeof(Component).IsAssignableFrom(type))
            {
                Debug.LogWarning($"[UIBase] Type {type.Name} is not a valid UI Component.");
                continue;
            }

            Component[] components = GetComponentsInChildren(type, true);

            if (!_typedCache.TryGetValue(type, out var dict))
                dict = _typedCache[type] = new Dictionary<string, Component>();

            foreach (var comp in components)
            {
                string key = comp.gameObject.name;
                if (!dict.ContainsKey(key))
                    dict[key] = comp;
            }
        }
    }

    protected void CacheAllUI(List<Type> types)
    {
        CacheAllUI(types.ToArray());
    }

    protected void CacheAllUI<T>() where T : Component
    {
        CacheAllUI(new Type[] { typeof(T) });
    }
}
