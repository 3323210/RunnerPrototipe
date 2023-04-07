using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object treadLock = new object();
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            lock (treadLock)
            {
                T[] instances = FindObjectsOfType<T>();
                if (instances.Length > 0)
                {
                    instance = instances[0];
                    for (int i = 1; 1 < instances.Length; i++)
                    {
                        Destroy(instances[i]);
                    }
                }
                else
                {
                    GameObject go = new GameObject();
                    go.name = typeof(T).ToString();
                    instance = go.AddComponent<T>();
                }
                DontDestroyOnLoad(instance);
                return instance;
            }

        }

    }
}