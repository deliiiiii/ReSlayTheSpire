using System;
using JetBrains.Annotations;
using UnityEngine.Events;

public class BindDataEvent : BindDataBase
{
    [CanBeNull] UnityAction act;
    readonly UnityEvent evt;

    public BindDataEvent(UnityEvent evt)
    {
        this.evt = evt;
    }

    public BindDataEvent To(UnityAction fAct)
    {
        if(act != null)
            evt.RemoveListener(act);
        act = fAct;
        return this;
    }

    public override void Bind()
    {
        evt.AddListener(act);
    }

    public override void UnBind()
    {
        if(act != null)
            evt.RemoveListener(act);
    }
    
}