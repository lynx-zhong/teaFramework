public abstract class Singleton<T> where T : class, new()
{
    private static T instance = null;

    private static readonly object locker = new object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                        instance = new T();
                }
            }

            return instance;
        }
    }
}
