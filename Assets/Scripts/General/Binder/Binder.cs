using System;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Binder
{
    public static BindDataAct<T> From<T>(Observable<T> osv) 
    {
        return new BindDataAct<T>(osv);
    }
    public static BindDataEvent From(Button btn)
    {
        return new BindDataEvent(btn.onClick);
    }
    
    public static BindDataEvent From(UnityEvent evt)
    {
        return new BindDataEvent(evt);
    }
    
    public static BindDataEvent<T> From<T>(UnityEvent<T> evt)
    {
        return new BindDataEvent<T>(evt);
    }

    [Obsolete]
    public static BindState From(MyState state)
    {
        return new BindState(state);
    }

    public static BindDataUpdate FromUpdate(Action<float> act, EUpdatePri priority = EUpdatePri.Default)
    {
        return new BindDataUpdate(act, priority);
    }

    public static BindDataUpdate FromUpdate<TEnum>(Action<float> act, TEnum priority)
    {
        return new BindDataUpdate(act, (EUpdatePri)Convert.ToInt32(priority));
    }
    
    [Obsolete]
    public static BindDataUpdate Update(Action<float> act, EUpdatePri priority = EUpdatePri.Default)
    {
        var ret = new BindDataUpdate(act, priority);
        ret.Bind();
        return ret;
    }
}

