using System;
using System.Collections.Generic;

namespace Violee;

[Serializable]
public class MyList<T>(IEnumerable<T> ie, Action<T>? onAdd = null, Action<T>? onRemove = null)
    : List<T>(ie)
{
    public event Action<T>? OnAdd = onAdd;
    public event Action<T>? OnRemove = onRemove;
    
    /// Please Call MyAdd() instead
    public new void Add(T item)
    {
        MyDebug.LogError("Please Call MyAdd() instead");
    }
    
    /// Please Call MyRemove() instead
    public new bool Remove(T item)
    {
        MyDebug.LogError("Please Call MyRemove() instead");
        return false;
    }
    
    /// Please Call MyClear() instead
    public new void Clear()
    {
        MyDebug.LogError("Please Call MyClear() instead");
    }
    
    public void MyAdd(T item)
    {
        base.Add(item);
        OnAdd?.Invoke(item);
    }
    public void MyRemove(T item)
    {
        base.Remove(item);
        OnRemove?.Invoke(item);
    }
    
    public void MyClear()
    {
        foreach (var item in this)
            OnRemove?.Invoke(item);
        Clear();
    }

    public MyList(Action<T>? onAdd = null, Action<T>? onRemove = null) : this([], onAdd, onRemove)
    {
    }
}