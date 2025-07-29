using System;
using System.Collections.Generic;
using JetBrains.Annotations;

public class BindDataUpdate
{
    public Action<float> Act;
    readonly EUpdatePri priority;
    public HashSet<Func<bool>> GuardSet = new ();

    public BindDataUpdate(Action<float> act, EUpdatePri priority)
    {
        this.Act = act;
        this.priority = priority;
    }
    
    public BindDataUpdate Where(Func<bool> guard)
    {
        GuardSet.Add(guard);
        return this;
    }

    public void UnBind()
    {
        if(Updater.UpdateDic.TryGetValue(priority, out var value))
            value.Remove(this);
    }
}

public enum EUpdatePri
{
    Default = -1,
    MainModel = 0,
    // P1 = 1,
    // P2 = 2,
}