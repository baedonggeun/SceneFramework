using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    private static bool _applicationIsQuitting = false;

    //싱글톤 인스턴스 접근
    public static T Instance
    {
        get
        {
            if(_applicationIsQuitting)
            {
                Debug.LogWarning($"[MonoSingleton] Instance '{typeof(T)}' already destroyed on application quit.");
                return null;
            }

            if(_instance == null)
            {
                CreateInstance();
            }

            return _instance;
        }
    }

    //인스턴스가 유효하고 앱 종류 중이 아닌지 확인
    public static bool IsInitialized => _instance != null && !_applicationIsQuitting;

    //외부에서 강제로 생성
    public static void ForceCreate()
    {
        if (_instance == null) CreateInstance();
    }

    //인스턴스 초기화 해제 및 파괴(테스트용)
    public static void ResetInstance()
    {
        if (_instance != null)
        {
            _instance.OnSingletonDestroyed();

            if(Application.isPlaying)   Destroy(_instance.gameObject);
            else DestroyImmediate(_instance.gameObject);

            _instance = null;
            _applicationIsQuitting = false;
        }
    }

    //파괴 전에 정리할 것들(자식에서 override 가능)
    protected virtual void OnSingletonDestroyed()
    {

    }

    //사용자 정의 초기화 로직(자식에서 override 가능)
    protected virtual void InitSingleton()
    {

    }

    //실제 인스턴스를 생성하는 로직
    private static void CreateInstance()
    {
        T[] objects = FindObjectsOfType<T>();

        if(objects.Length > 0)
        {
            _instance = objects[0];

            for(int i = 1; i < objects.Length; i++)
            {
                if (Application.isPlaying) Destroy(objects[i].gameObject);
                else DestroyImmediate(objects[i].gameObject);
            }
        }
        else
        {
            GameObject go = new GameObject(typeof(T).Name);
            _instance = go.AddComponent<T>();

            DontDestroyOnLoad(go);
        }

        _instance.InitSingleton();
    }

    //Awake에서 인스턴스 할당 및 중복 제거 - Not used in this architecture
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            InitSingleton();
        }
        else if (_instance != this)
        {
            if (Application.isPlaying) Destroy(gameObject);
            else DestroyImmediate(gameObject);
        }
    }

    //애플리케이션 종료 시 처리
    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    //오브젝트 파괴 시 처리
    protected virtual void OnDestroy()
    {
        if(_instance == this)
        {
            OnSingletonDestroyed();
            _instance = null;
        }
    }
}