using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

public class BindDataUpdate
{
    public Action<float> Act;
    public readonly EUpdatePri Priority;
    public HashSet<Func<bool>> GuardSet = new ();

    public BindDataUpdate(Action<float> act, EUpdatePri priority)
    {
        this.Act = act;
        this.Priority = priority;
    }
    
    public BindDataUpdate Where(Func<bool> guard)
    {
        GuardSet.Add(guard);
        return this;
    }

    public void UnBind()
    {
        if(Updater.UpdateDic.TryGetValue(Priority, out var value))
            value.Remove(this);
    }
}

public enum EUpdatePri
{
    Default = -1,
    Input,
    Sprite,
}