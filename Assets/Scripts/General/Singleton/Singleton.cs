using System.Diagnostics.CodeAnalysis;
using UnityEngine;
//Mono单例
//需要被继承 xxx : Singleton<xxx>
//获取单例 xxx.Instance
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public bool GlobalOnScene;

    [field: AllowNull, MaybeNull]
    protected static T Instance
    {
        get
        {
            field ??= FindObjectOfType<T>();
            field ??= new GameObject().AddComponent<T>(); 
            return field;
        }
    }

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
    public static T? Instance
    {
        get
        {
            field ??= new T();
            field.OnInit();
            return field;
        }
    }
    protected virtual void OnInit()
    {
    }
}