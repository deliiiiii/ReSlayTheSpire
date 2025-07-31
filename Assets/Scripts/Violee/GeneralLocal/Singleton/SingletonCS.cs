using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Violee;

//C#单例
//需要被继承 xxx : SingletonCS<xxx>
//获取单例 xxx.Instance
// ReSharper disable once InconsistentNaming
public class SingletonCS<T> where T : SingletonCS<T>, new()
{
    protected GameObject go = null!;
    [field: AllowNull, MaybeNull]
    protected static T Instance
    {
        get
        {
            field ??= new T();
            field.OnInit();
            return field;
        }
    } = null!;
    void OnInit()
    {
        go = new GameObject($"{typeof(T).Name} (SingletonCS)");
    }
}


