using System;
using UnityEngine.Events;

public class BindDataEvent(UnityEvent evt)
{
    UnityAction act;

    public BindDataEvent To(UnityAction act)
    {
        evt.AddListener(act);
        this.act = act;
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

    public void UnBind()
    {
        evt.RemoveListener(act);
    }
    
}