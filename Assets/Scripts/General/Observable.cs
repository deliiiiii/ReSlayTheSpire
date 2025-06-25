using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Observable<T> where T: IComparable
{
    [SerializeField]
    T value;
    public T Value
    {
        get => value;
        set
        {
            if (Equals(value, this.value))
            {
                return;
            }
            // OnValueChangedBefore?.Invoke(_value);
            this.value = value;
            OnValueChangedAfter?.Invoke(this.value);
        }
    }
    // public event UnityAction<T> OnValueChangedBefore;
    public event UnityAction<T> OnValueChangedAfter;
    public Observable(T initValue)
    {
        value = initValue;
    }
    // public Observable(T initValue, UnityAction<T> before, UnityAction<T> after)
    public Observable(T initValue, UnityAction<T> after)
    {
        value = initValue;
        // OnValueChangedBefore += before;
        OnValueChangedAfter += after;
    }
    public static implicit operator T(Observable<T> v)
    {
        return v.Value;
    }
    
    public static implicit operator float(Observable<T> v)
    {
        return v.Value switch
        {
            int i => i,
            float f => f,
            double d => (float)d,
            char c => c,
            _ => (dynamic)v.value
        };
    }


    public override string ToString()
    {
        return value.ToString();
    }
}




