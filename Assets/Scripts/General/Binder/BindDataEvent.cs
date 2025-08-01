using System;
using JetBrains.Annotations;
using UnityEngine.Events;

public class BindDataEvent
{
    [CanBeNull] UnityAction act;
    [CanBeNull] UnityAction lastAct;
    readonly UnityEvent evt;

    public BindDataEvent(UnityEvent evt)
    {
        this.evt = evt;
    }

    public BindDataEvent To(UnityAction fAct)
    {
        if(lastAct != null)
            evt.RemoveListener(lastAct);
        lastAct = fAct;
        evt.AddListener(fAct);
        act = fAct;
        return this;
    }
    
    /// 不允许多个
    public BindDataEvent AnotherTo(UnityAction fAct)
    {
        evt.AddListener(fAct);
        return this;
    }

    void UnBindAll()
    {
        evt.RemoveAllListeners();
    }

    public void UnBind()
    {
        evt.RemoveListener(act);
    }
    
}