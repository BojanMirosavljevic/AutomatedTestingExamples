using UnityEngine;

public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;
}

public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            base.Awake();
        }
    }
}

public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected bool initialized;
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Init();
    }

    protected void Init()
    {
        if (!initialized)
        {
            OnInitialize();
            initialized = true;
        }
    }

    protected virtual void OnInitialize()
    {
        
    }

    public bool IsInitialized()
    {
        return initialized;
    }
}

