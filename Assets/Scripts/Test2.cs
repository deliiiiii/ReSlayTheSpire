using UnityEngine;
using System.ComponentModel;
using PropertyChanged;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

// #region AddINotifyPropertyChangedInterface
// public class Test : MonoBehaviour
// {
//     TestProperty testProperty = new TestProperty();
//     void Update()
     // {
     //     if (Input.GetKeyDown(KeyCode.A))
     //     {
     //         testProperty.IntProperty1++;
     //     }
//     }
// }
//
// [AddINotifyPropertyChangedInterface]
// public class TestProperty
// {
//     // [AlsoNotifyFor(nameof(IntProperty2))]
//     public int IntProperty1 { get; set; }
//
//     [DoNotNotify]
//     public int IntProperty2 => IntProperty1 + 1;
//     void OnIntProperty1Changed()
//     {
//         Debug.Log($"[Fody-Callback] IntProperty1 -> {IntProperty1}");
//     }
//     void OnIntProperty2Changed()
//     {
//         Debug.Log($"[Fody-Callback] IntProperty2 -> {IntProperty2}");
//     }
// }
//
// #endregion



[Serializable]
public class Observable<T>
{
    [SerializeField]
    T _value;
    public T Value
    {
        get => _value;
        set
        {
            if (Equals(value, _value))
            {
                return;
            }

            onValueChangedBefore?.Invoke(_value);
            _value = value;
            onValueChangedAfter?.Invoke(_value);
        }
    }
    public event Action<T> onValueChangedBefore;
    public event Action<T> onValueChangedAfter;

    public Observable(T initValue, Action<T> initEvent = null)
    {
        _value = initValue;
        onValueChangedAfter += initEvent;
        Dic = new Dictionary<object, Action<T>>();
    }

    public Dictionary<object, Action<T>> Dic;

}


public static class Binder
{
    //
    // public static void Bind<T1, T2>(Observable<T1> from, Observable<T2> to, Func<T1, T2> converter)
    // {
    //     if (from.Dic.ContainsKey(to))
    //     {
    //         Debug.LogWarning($"Binding already exists for this source:{from} and target:{to}.");
    //         Unbind(from, to);
    //     }
    //     Action<T1> ac = (newValue) =>
    //     {
    //         to.Value = converter(newValue);
    //     };
    //     from.onValueChanged += ac;
    //     from.Dic[to] = ac;
    // }

    
    // public static void Bind<T1, T2>(Observable<T1> from,ref T2 to, Func<T1, T2> converter)
    // {
    //     if (from.Dic.ContainsKey(to))
    //     {
    //         Debug.LogWarning($"Binding already exists for this source:{from} and target:{to}.");
    //         Unbind(from,ref to);
    //     }
    //     Action<T1> ac = (newValue) =>
    //     {
    //         to = converter(newValue);
    //     };
    //     from.onValueChanged += ac;
    //     from.Dic[to] = ac;
    // }
    
    
    public static void BindAction<T1>(Observable<T1> from, Action action)
    {
        from.onValueChangedAfter += (T1 t1) => action();
    }

    public static void BindText<T1>(Observable<T1> from, Text text)
    {
        from.onValueChangedAfter += (T1 t1) =>
        {
            text.text = from.Value.ToString();
        };
    }

    
    
    // public static void Unbind<T1, T2>(Observable<T1> from, Observable<T2> to)
    // {
    //     if (from.Dic.TryGetValue(to, out var v))
    //     {
    //         from.onValueChanged -= v;
    //         from.Dic.Remove(v);
    //     }
    // }
    //
    // public static void Unbind<T1, T2>(Observable<T1> from, ref T2 to)
    // {
    //     if (from.Dic.TryGetValue(to, out var v))
    //     {
    //         from.onValueChanged -= v;
    //         from.Dic.Remove(v);
    //     }
    // }

}


[Serializable]
public class Model1
{
    public Observable<int> I = new Observable<int>(66);
}


public class Test2 : MonoBehaviour
{
    public Model1 Model1Instance = new Model1();
    float f = 111f;
    public Text TestText;

    void Awake()
    {
        Binder.BindText(Model1Instance.I, TestText);
    }

    void Update()
    {
    }
}