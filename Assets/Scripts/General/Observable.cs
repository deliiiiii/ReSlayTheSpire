using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Observable<T> where T: IComparable
{
    [SerializeField] [CanBeNull] T value;
    [CanBeNull]
    public T Value
    {
        get => value;
        set
        {
            var oldV = this.value;
            if (value?.CompareTo(oldV) == 0)
            {
                return;
            }
            OnValueChangedBefore?.Invoke(oldV);
            this.value = value;
            OnValueChangedAfter?.Invoke(this.value);
            // OnValueChangedFull?.Invoke(oldV, value);
        }
    }
    [CanBeNull] public event UnityAction<T> OnValueChangedBefore;
    [CanBeNull] public event UnityAction<T> OnValueChangedAfter;
    // [CanBeNull] public event UnityAction<T, T> OnValueChangedFull;
    public Observable(T initValue)
    {
        value = initValue;
    }
    public Observable(T initValue, [CanBeNull] UnityAction<T> before = null, [CanBeNull] UnityAction<T> after = null)
    {
        value = initValue;
        OnValueChangedBefore += before;
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
        return value?.ToString() ?? $"NULL Observable{typeof(T)}";
    }
}




