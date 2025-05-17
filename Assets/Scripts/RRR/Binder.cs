using System;
using UnityEngine;
using UnityEngine.UI;

public static class Binder
{
    public static BindDataAct<T> From<T>(Observable<T> osv) where T : IComparable
    {
        return new BindDataAct<T>(osv);
    }
    public static BindDataBtn From(Button btn)
    {
        return new BindDataBtn(btn);
    }
    public static BindDataBtn From(GameObject pnl)
    {
        return From(pnl.GetComponent<Button>());
    }

    public static BindDataState From(MyState state)
    {
        return new BindDataState(state);
    }
    
}