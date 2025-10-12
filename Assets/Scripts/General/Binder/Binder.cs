using System;
using UnityEngine.UI;

public static class Binder
{
    public static BindDataAct<T> From<T>(Observable<T> osv) 
    {
        return new BindDataAct<T>(osv);
    }
    // public static BindDataActComparable<T> FromComparable<T>(Observable<T> osv) where T : IComparable
    // {
    //     return new BindDataActComparable<T>(osv);
    // }
    public static BindDataEvent From(Button btn)
    {
        return new BindDataEvent(btn.onClick);
    }
    // public static BindDataEvent From(GameObject pnl)
    // {
    //     return From(pnl.GetComponent<Button>());
    // }

    public static BindDataState From(MyState state)
    {
        return new BindDataState(state);
    }

    public static BindDataUpdate From(Action<float> act, EUpdatePri priority = EUpdatePri.Default)
    {
        return new BindDataUpdate(act, priority);
    }

    public static BindDataUpdate From<TEnum>(Action<float> act, TEnum priority)
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

