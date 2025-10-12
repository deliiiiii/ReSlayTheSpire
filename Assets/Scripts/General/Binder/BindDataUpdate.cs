using System;
using System.Collections.Generic;

public class BindDataUpdate
{
    public Action<float> Act;
    public readonly int Priority;
    public HashSet<Func<bool>> GuardSet = new ();

    public BindDataUpdate(Action<float> act, EUpdatePri priority)
    {
        this.Act = act;
        this.Priority = (int)priority;
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

public class BindDataUpdate<T> : BindDataUpdate
    where T : Enum
{
    public BindDataUpdate(Action<float> act, T priority) : base(act, (EUpdatePri)Convert.ToInt32(priority)){}
}

public enum EUpdatePri
{
    Default = -1,
    Input,
    
    Fsm = 10,
    
    Sprite = 11,
}