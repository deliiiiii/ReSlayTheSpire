using UnityEngine;
using System.ComponentModel;
using PropertyChanged;
using System;
using System.Collections.Generic;

#region AddINotifyPropertyChangedInterface
// public class Test : MonoBehaviour
// {
//     TestProperty testProperty = new TestProperty();
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.A))
//         {
//             testProperty.IntProperty1++;
//         }
//     }
// }

[AddINotifyPropertyChangedInterface]
public class TestProperty
{
    // [AlsoNotifyFor(nameof(IntProperty2))]
    public int IntProperty1 { get; set; }

    [DoNotNotify]
    public int IntProperty2 => IntProperty1 + 1;
    void OnIntProperty1Changed()
    {
        Debug.Log($"[Fody-Callback] IntProperty1 -> {IntProperty1}");
    }
    void OnIntProperty2Changed()
    {
        Debug.Log($"[Fody-Callback] IntProperty2 -> {IntProperty2}");
    }
}

#endregion



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
            onValueChanged?.Invoke(value);
            _value = value;
        }
    }
    public event Action<T> onValueChanged;

    public Observable(T initValue, Action<T> initEvent = null)
    {
        _value = initValue;
        onValueChanged += initEvent;
        Dic = new Dictionary<object, Action<T>>();
    }

    public Dictionary<object, Action<T>> Dic;

}


public static class BindStaticClass
{

    public static void Bind<T1, T2>(Observable<T1> from, Observable<T2> to, Func<T1, T2> converter)
    {
        if (from.Dic.ContainsKey(to))
        {
            Debug.LogWarning($"Binding already exists for this source:{from} and target:{to}.");
            Unbind(from, to);
        }
        Action<T1> ac = (newValue) =>
        {
            to.Value = converter(newValue);
        };
        from.onValueChanged += ac;
        from.Dic[to] = ac;
    }
    public static void Unbind<T1, T2>(Observable<T1> from, Observable<T2> to)
    {
        if (from.Dic.TryGetValue(to, out var v))
        {
            from.onValueChanged -= v;
            from.Dic.Remove(v);
        }

        
        
    }

}

public static class IntModifier
{
    public static Func<int, float> Func1p1 = (int v) => v * 1.1f;
}

[Serializable]
public class Model1
{
    public Observable<int> I = new Observable<int>(66);
}

[Serializable]
public class Model2
{
    public Observable<float> F = new Observable<float>(33.3f);
}

public class Test2 : MonoBehaviour
{
    public Model1 Model1Instance = new Model1();
    public Model2 Model2Instance = new Model2();


    // public Observable<bool> Input;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Model1Instance.I.Value += 1;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            BindStaticClass
                .Bind(Model1Instance.I, Model2Instance.F, IntModifier.Func1p1);
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            BindStaticClass
                .Unbind(Model1Instance.I, Model2Instance.F);
        }
    }
}

// BindStaticClass.From(Model1Instance.I).To(Model2Instance.F).
//