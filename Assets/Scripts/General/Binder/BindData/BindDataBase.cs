using System;
using System.Collections.Generic;

public abstract class BindDataBase
{
    public void Bind()
    {
        foreach (var guard in GuardSet)
        {
            if (!guard.Invoke())
            {
                return;
            }
        }
        BindInternal();
    }
    protected abstract void BindInternal();
    public abstract void UnBind(); 
    public readonly HashSet<Func<bool>> GuardSet = new ();
    public T Where<T>(Func<bool> guard)
        where T : BindDataBase
    {
        GuardSet.Add(guard);
        return this as T;
    }
}


