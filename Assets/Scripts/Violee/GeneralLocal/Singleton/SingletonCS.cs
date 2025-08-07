using System.Diagnostics.CodeAnalysis;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
            if (field == null)
            {
                field = new T();
                field.OnInit();
            }
            return field;
        }
    } = null!;
    void OnInit()
    {
        var destroyAct = () =>
        {
            Object.DestroyImmediate(go);
            go = null!;
        };
        go = new GameObject($"{typeof(T).Name} (SingletonCS)");
#if UNITY_EDITOR
        AssemblyReloadEvents.beforeAssemblyReload += () => destroyAct();
        EditorApplication.playModeStateChanged += s =>
        {
            if (s == PlayModeStateChange.ExitingPlayMode && go != null)
            {
                destroyAct();
            }
        };
#endif
    }
}


