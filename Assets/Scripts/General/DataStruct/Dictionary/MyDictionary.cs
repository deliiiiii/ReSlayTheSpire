using System;
using System.Collections.Generic;

[Serializable]
public class MyDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    public event Action<TValue> OnAdd;
    public event Action<TValue> OnRemove;

    public new void Add(TKey key, TValue value)
    {
        base.Add(key, value);
        OnAdd?.Invoke(value);
    }

    public new bool Remove(TKey key)
    {
        if (ContainsKey(key))
        {
            OnRemove?.Invoke(base[key]);
            base.Remove(key);
            return true;
        }
        return false;
    }
}