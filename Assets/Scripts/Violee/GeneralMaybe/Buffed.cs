using System;
using UnityEngine;

[Serializable]
public class Buffed<T>
{
    [SerializeField]
    T value;
    [NonSerialized] Func<T, T>? buffFunc;
    public T Value 
        => buffFunc == null ? value : buffFunc(value);

    public Buffed()
    {
        value = default!;
    }
    public Buffed(T initValue, Func<T, T>? buffFunc = null)
    {
        value = initValue;
    }
    
    public void SetBuff(Func<T, T> func)
    {
        buffFunc = func;
    }

    public static implicit operator T(Buffed<T> buffed)
    {
        return buffed.Value;
    }
    
    public override string ToString()
    {
        return Value?.ToString() ?? $"NULL Buffed{typeof(T)}";
    }
}

[Serializable]
public class BuffedInt(int initValue, Func<int, int>? buffFunc = null) : Buffed<int>(initValue, buffFunc);
[Serializable]
public class BuffedFloat(float initValue, Func<float, float>? buffFunc = null) : Buffed<float>(initValue, buffFunc);