using UnityEngine;
using System.ComponentModel;
using PropertyChanged;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
        // get => _value;
        get => _value;
        set
        {
            if (Equals(value, _value))
            {
                return;
            }
            OnValueChangedBefore?.Invoke(_value);
            _value = value;
            OnValueChangedAfter?.Invoke(_value);
        }
    }
    public event Action<T> OnValueChangedBefore;
    public event Action<T> OnValueChangedAfter;

    public void Immediate()
    {
        OnValueChangedAfter?.Invoke(_value);
    }
    public Observable(T initValue, Action<T> initEvent = null)
    {
        _value = initValue;
        OnValueChangedAfter += initEvent;
        // Dic = new Dictionary<object, Action<T>>();
    }
    // public Dictionary<object, Action<T>> Dic;
    public static implicit operator T(Observable<T> v)
    {
        return v.Value;
    }
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


    public static void BindChange<T1>(Observable<T1> from, Action action, bool immediate = false)
    {
        // from.onValueChangedAfter += (T1 t1) => action();
        from.OnValueChangedAfter += _ => action();
        if (immediate)
        {
            from.Immediate();
        }
    }

    public static void BindChange<T1>(Observable<T1> from, Text text, bool immediate = false)
    {
        BindChange(from, () => text.text = from.Value.ToString(), immediate);
    }

    public static void BindCulminate(Observable<float> from, float threshold, Action action, bool immediate = false) 
    {
        BindChange(from, () =>
        {
            if (from.Value.CompareTo(threshold) < 0)
                return;
            from.Value -= threshold;
            action();
        },
        immediate);
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


public class Trigger
{
    public Func<bool> TriggerEvt;
    public Func<bool> Condition;
    public Action TrueResult;
    public Action FalseResult;

    public Trigger(Func<bool> triggerEvt, Func<bool> condition, Action trueResult, Action falseResult)
    {
        TriggerEvt = triggerEvt;
        Condition = condition;
        TrueResult = trueResult;
        FalseResult = falseResult;
    }

    public Trigger(Func<bool> condition, Action trueResult)
    {
        Condition = condition;
        TrueResult = trueResult;
    }
    public Trigger(Func<bool> condition, Action trueResult, Action falseResult)
    {
        Condition = condition;
        TrueResult = trueResult;
        FalseResult = falseResult;
    }
}