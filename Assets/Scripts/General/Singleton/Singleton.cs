using UnityEngine;
//Mono单例
//需要被继承 xxx : Singleton<xxx>
//获取单例 xxx.Instance
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public bool GlobalOnScene = false;
    public static T Instance { get; private set; }

    void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this as T;
        // Instance!.OnInit();
        if (GlobalOnScene)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    public virtual void OnInit(){}
}


//C#单例
//需要被继承 xxx : SingletonCS<xxx>
//获取单例 xxx.Instance
// ReSharper disable once InconsistentNaming
public class SingletonCS<T> where T : SingletonCS<T>, new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            instance ??= new T();
            instance.OnInit();
            return instance;
        }
    }
    protected virtual void OnInit()
    {
    }
}