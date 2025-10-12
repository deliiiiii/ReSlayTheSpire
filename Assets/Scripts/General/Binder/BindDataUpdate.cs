using System;
using System.Collections.Generic;
using System.Linq;

public class BindDataUpdate : BindDataBase
{
    public readonly Action<float> Act;
    public readonly HashSet<Func<bool>> GuardSet = new ();
    readonly int priority;

    public BindDataUpdate(Action<float> act, EUpdatePri e)
    {
        Act = act;
        priority = (int)e;
    }
    
    public override void Bind()
    {
        Updater.UpdateDic.TryAdd(priority, new HashSet<BindDataUpdate>());
        Updater.UpdateDic[priority].Add(this);
    }
    
    public override void UnBind()
    {
        // if (Updater.UpdateDic.TryGetValue(priority, out var value))
        // {
            var found = Updater.UpdateDic[priority].FirstOrDefault(v => v.Act == Act);
            if(found != null)
                Updater.UpdateDic[priority].Remove(found);
        // }
    }
    
    public BindDataUpdate Where(Func<bool> guard)
    {
        GuardSet.Add(guard);
        return this;
    }
}

public enum EUpdatePri
{
    Default = -1,
    Input,
    
    Fsm = 10,
    
    Sprite = 11,
}