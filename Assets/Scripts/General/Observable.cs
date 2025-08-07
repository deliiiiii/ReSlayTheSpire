using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Observable<T>
{
    [SerializeField] [CanBeNull] T value;
    [CanBeNull]
    public T Value
    {
        get => value;
        set
        {
            var oldV = this.value;
            if (value is IComparable com)
            {
                if (com.CompareTo(oldV) == 0)
                {
                    return;
                }
            }
            else if (value != null && value.Equals(oldV))
                return;
            OnValueChangedBefore?.Invoke(oldV);
            this.value = value;
            OnValueChangedAfter?.Invoke(this.value);
            OnValueChangedFull?.Invoke(oldV, value);
        }
    }
    [CanBeNull] public event UnityAction<T> OnValueChangedBefore;
    [CanBeNull] public event UnityAction<T> OnValueChangedAfter;
    [CanBeNull] public event UnityAction<T, T> OnValueChangedFull;
    [JsonConstructor]
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




