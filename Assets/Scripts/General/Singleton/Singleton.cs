using UnityEngine;
//Mono单例
//需要被继承 xxx : Singleton<xxx>
//获取单例 xxx.Instance
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T instance;
    public bool globalOnScene = false;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                instance.OnInit();
            }
            return instance;
        }
    }
    private void Awake()
    {
       if (globalOnScene)
       {
           DontDestroyOnLoad(gameObject);
       }
    }
    protected virtual void OnInit()
    {
    }
}


//C#单例
//需要被继承 xxx : SingletonCS<xxx>
//获取单例 xxx.Instance
public class SingletonCS<T> where T : SingletonCS<T>, new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            instance ??= new();
            instance.OnInit();
            return instance;
        }
    }
    protected virtual void OnInit()
    {
    }
}