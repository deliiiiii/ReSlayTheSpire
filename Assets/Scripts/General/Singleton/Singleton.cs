using UnityEngine;
//Mono单例
//需要被继承 xxx : Singleton<xxx>
//获取单例 xxx.Instance
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T instance;
    public bool GlobalOnScene;

    public static T Instance => instance ? instance : FindObjectOfType<T>() ? FindObjectOfType<T>() : new GameObject().AddComponent<T>();

    protected virtual void Awake()
    {
        if(name == "New Game Object")
            name = GetType().ToString();
        if(Instance && Instance != this)
        {
            // duplicate!!!
            Destroy(Instance.gameObject);
        }
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
    static T instance;
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