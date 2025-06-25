using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public static class Binder
{
    public static BindDataAct<T> From<T>(Observable<T> osv) where T : IComparable
    {
        return new BindDataAct<T>(osv);
    }
    public static BindDataEvent From(Button btn)
    {
        return new BindDataEvent(btn.onClick);
    }
    public static BindDataEvent From(GameObject pnl)
    {
        return From(pnl.GetComponent<Button>());
    }

    public static BindDataState From(MyState state)
    {
        return new BindDataState(state);
    }
    
    public static BindDataUpdate Update(Action<float> act, EUpdatePri priority)
    {
        var ret = new BindDataUpdate(act, priority);
        if(!Updater.UpdateDic.ContainsKey(priority))
            Updater.UpdateDic.Add(priority, new HashSet<BindDataUpdate>());
        Updater.UpdateDic[priority].Add(ret);
        return ret;
    }
}

