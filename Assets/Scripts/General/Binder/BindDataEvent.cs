using System;
using UnityEngine.Events;

public class BindDataEvent
{
    readonly UnityEvent evt;
    public BindDataEvent(UnityEvent evt)
    {
        this.evt = evt;
    }
    
    public BindDataEvent To(UnityAction act)
    {
        UnBindAll();
        evt.AddListener(act);
        return this;
    }
    
    // TODO 不允许多个?
    public BindDataEvent AnotherTo(UnityAction act)
    {
        evt.AddListener(act);
        return this;
    }

    void UnBindAll()
    {
        evt.RemoveAllListeners();
    }
    
    //TODO UnBind
    
}