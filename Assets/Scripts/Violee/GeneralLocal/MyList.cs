using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.Utilities;

namespace Violee;

[Serializable]
public class MyList<T>(IEnumerable<T> ie, Action<T>? onAdd = null, Action<T>? onRemove = null)
    : List<T>(ie)
{
    [JsonConstructor]
    public MyList() : this(new List<T>(), null, null) { }
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

    public new int RemoveAll(Predicate<T> match)
    {
        MyDebug.LogError("Please Call MyRemoveAll() instead");
        return 0;
    }

    public void MyRemoveAll(Predicate<T> match)
    {
        var allRemoved = this.Where(x => match(x));
        allRemoved.ForEach(MyRemove);
    }
    
    public void MyClear()
    {
        foreach (var item in this)
            OnRemove?.Invoke(item);
        base.Clear();
    }

    public MyList(Action<T>? onAdd = null, Action<T>? onRemove = null) : this([], onAdd, onRemove)
    {
    }
}