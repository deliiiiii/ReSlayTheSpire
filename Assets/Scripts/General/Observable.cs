#nullable enable
using System;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Observable<T>
{
    [SerializeField] T? value;
    bool forceEverySet;
    bool enableRepeatEvent;

    bool canAddOne => value is int or float or double;
    [Button][ShowIf(nameof(canAddOne))]
    public void AddOne()
    {
        if (canAddOne)
        {
            Value = (Value as dynamic) + 1;
        }
    }
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
    UnityAction<T?>? onValueChangedBefore;
    public event UnityAction<T>? OnValueChangedBefore
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
    
    UnityAction<T?>? onValueChangedAfter;
    public event UnityAction<T?>? OnValueChangedAfter
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
    UnityAction<T?, T?>? onValueChangedFull;
    public event UnityAction<T?, T?>? OnValueChangedFull
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
    public Observable(T? initValue, 
        UnityAction<T?>? before = null, 
        UnityAction<T?>? after = null,
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
        return v.Value!;
    }
    
    public static implicit operator float(Observable<T> v)
    {
        return v.Value switch
        {
            int i => i,
            float f => f,
            double d => (float)d,
            char c => c,
            _ => throw new InvalidCastException($"Cannot convert {v.value} (Type: {typeof(T)}) to float")
        };
    }


    public override string ToString()
    {
        return Value?.ToString() ?? $"NULL Observable{typeof(T)}";
    }
}


// [Serializable]
// public class ObservableInt : Observable<int>
// {
//     public ObservableInt(int initValue) : base(initValue)
//     {
//     }
//
//     public ObservableInt(int initValue, [CanBeNull] UnityAction<int> before = null, [CanBeNull] UnityAction<int> after = null, bool forceEverySet = false, bool enableRepeatEvent = false) : base(initValue, before, after, forceEverySet, enableRepeatEvent)
//     {
//     }
// }
//
// [Serializable]
// public class ObservableFloat : Observable<float>
// {
//     public ObservableFloat(float initValue) : base(initValue)
//     {
//     }
//
//     public ObservableFloat(float initValue, [CanBeNull] UnityAction<float> before = null, [CanBeNull] UnityAction<float> after = null, bool forceEverySet = false, bool enableRepeatEvent = false) : base(initValue, before, after, forceEverySet, enableRepeatEvent)
//     {
//     }
// }
//
// [Serializable]
// public class ObservableBool : Observable<bool>
// {
//     public ObservableBool(bool initValue) : base(initValue)
//     {
//     }
//
//     public ObservableBool(bool initValue, [CanBeNull] UnityAction<bool> before = null, [CanBeNull] UnityAction<bool> after = null, bool forceEverySet = false, bool enableRepeatEvent = false) : base(initValue, before, after, forceEverySet, enableRepeatEvent)
//     {
//     }
// }

// [Serializable]
// public class ObservableEnum : Observable<Enum>
// {
//     public ObservableEnum(Enum initValue) : base(initValue)
//     {
//     }
//
//     public ObservableEnum(Enum initValue, [CanBeNull] UnityAction<Enum> before = null, [CanBeNull] UnityAction<Enum> after = null, bool forceEverySet = false, bool enableRepeatEvent = false) : base(initValue, before, after, forceEverySet, enableRepeatEvent)
//     {
//     }
// }