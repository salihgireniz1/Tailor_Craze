using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + " (Singleton)";

                    // Optionally, if you want the singleton to persist across scenes
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject); // Optional: Keep the instance persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy the duplicate instance
        }
    }

    // Optionally, you can add a method for manual cleanup if needed.
    public virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
