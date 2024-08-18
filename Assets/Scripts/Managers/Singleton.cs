using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>, IInitiable
{
    private static T instance;

    public static T Instance => instance;

    public static bool IsInitialized => instance != null;

    [SerializeField]
    [Tooltip("If is pesistant, will go to don't destroy on load.")]
    private bool _isPersistant = false;

    public virtual void Init() { }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Debug.Log(string.Format("[Singleton] Trying to instantiate a second instance of singleton class {0}", GetType().Name));

            if (_isPersistant)
                Destroy(gameObject);
            else
                Destroy(this);
        }
        else
        {
            instance = (T)this;

            if (_isPersistant)
            {
                transform.parent = null;
                //DontDestroyOnLoad(gameObject);
            }

            SingletonInitializer.Record(instance);
        }
    }

    protected void InitAllSingletons()
    {
        foreach (IInitiable singleton in SingletonInitializer.singletonList)
        {
            singleton.Init();
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            SingletonInitializer.Remove(instance);
            instance = null;
        }
    }

    protected void Log(string log)
    {
        Debug.Log("[" + gameObject.name + "] : " + log);
    }
}

public static class SingletonInitializer
{
    public static List<IInitiable> singletonList = new List<IInitiable>();

    public static void Record(IInitiable singleton)
    {
        if (singletonList == null)
        {
            singletonList = new List<IInitiable>();
        }

        if (!singletonList.Contains(singleton))
        {
            singletonList.Add(singleton);
        }
    }

    internal static void Remove<T>(T instance) where T : Singleton<T>, IInitiable
    {
        if (singletonList != null && singletonList.Contains(instance))
        {
            singletonList.Remove(instance);
        }
    }
}

public interface IInitiable
{
    public void Init();
}