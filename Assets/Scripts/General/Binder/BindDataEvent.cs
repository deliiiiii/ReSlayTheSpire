using System;
using UnityEngine.Events;

public class BindDataEvent
{
    UnityAction act;
    UnityAction lastAct;
    readonly UnityEvent evt;

    public BindDataEvent(UnityEvent evt)
    {
        this.evt = evt;
    }

    public BindDataEvent To(UnityAction fAct)
    {
        evt.RemoveListener(lastAct);
        lastAct = fAct;
        evt.AddListener(fAct);
        act = fAct;
        return this;
    }
    
    // TODO 不允许多个?
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