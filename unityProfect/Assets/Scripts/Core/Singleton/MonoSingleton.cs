using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object lockObj = new object();
    private static bool applicationIsQuitting;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                instance = null;
                return null;
            }

            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            return instance;
                        }

                        if (instance == null)
                        {
                            GameObject singleton = new GameObject();
                            instance = singleton.AddComponent<T>();
                            singleton.name = "(Singleton) " + typeof(T);
                            singleton.hideFlags = HideFlags.None;

                            DontDestroyOnLoad(singleton);
                        }
                        else
                        {
                            DontDestroyOnLoad(instance.gameObject);
                            instance.gameObject.name = "(Singleton) " + typeof(T);
                        }
                    }
                }
            }

            instance.hideFlags = HideFlags.None;
            return instance;
        }
    }

    protected virtual void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    protected virtual void Awake()
    {
        applicationIsQuitting = false;
    }

    public virtual T Init()
    {
        return Instance;
    }

    public virtual void Dispose()
    {
        Destroy(gameObject);
    }
}
