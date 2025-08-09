using System;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Observable<T>
{
    [SerializeField] [CanBeNull] T value;
    bool forceEverySet;
    bool enableRepeatEvent;

    [Button]
    public void AddOne()
    {
        if (value is int or float or double)
        {
            Value = (dynamic)Value + 1;
        }
    }
    [CanBeNull]
    public T? Value
    {
        get => value;
        set
        {
            var oldV = this.value;
            if (value is IComparable com)
            {
                if (com.CompareTo(oldV) == 0 && !forceEverySet)
                {
                    return;
                }
            }
            else if (value != null && value.Equals(oldV))
                return;
            onValueChangedBefore?.Invoke(oldV);
            this.value = value;
            onValueChangedAfter?.Invoke(this.value);
            onValueChangedFull?.Invoke(oldV, value);
        }
    }
    [CanBeNull] UnityAction<T> onValueChangedBefore;
    [CanBeNull] public event UnityAction<T> OnValueChangedBefore
    {
        add
        {
            if (!enableRepeatEvent || (!onValueChangedBefore?.GetInvocationList().Contains(value) ?? true))
            {
                onValueChangedBefore += value;
            }
        }
        remove => onValueChangedBefore -= value;
    }
    
    [CanBeNull] UnityAction<T> onValueChangedAfter;
    [CanBeNull] public event UnityAction<T> OnValueChangedAfter
    {
        add
        {
            if (!enableRepeatEvent || (!onValueChangedAfter?.GetInvocationList().Contains(value) ?? true))
            {
                onValueChangedAfter += value;
            }
        }
        remove => onValueChangedAfter -= value;
    }
    [CanBeNull] UnityAction<T, T> onValueChangedFull;
    [CanBeNull] public event UnityAction<T, T> OnValueChangedFull
    {
        add
        {
            if (!enableRepeatEvent || (!onValueChangedFull?.GetInvocationList().Contains(value) ?? true))
            {
                onValueChangedFull += value;
            }
        }
        remove => onValueChangedFull -= value;
    }
    [JsonConstructor]
    public Observable(T initValue)
    {
        value = initValue;
    }
    public Observable(T initValue, 
        [CanBeNull] UnityAction<T> before = null, 
        [CanBeNull] UnityAction<T> after = null,
        bool forceEverySet = false,
        bool enableRepeatEvent = false
        )
    {
        value = initValue;
        onValueChangedBefore += before;
        onValueChangedAfter += after;
        this.forceEverySet = forceEverySet;
        this.enableRepeatEvent = enableRepeatEvent;
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
            _ => (dynamic)v.Value
        };
    }


    public override string ToString()
    {
        return Value?.ToString() ?? $"NULL Observable{typeof(T)}";
    }
}




