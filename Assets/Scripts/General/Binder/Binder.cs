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

public class BindDataUpdate
{
    public Action<float> Act;
    readonly EUpdatePri priority;

    public BindDataUpdate(Action<float> act, EUpdatePri priority)
    {
        this.Act = act;
        this.priority = priority;
    }

    public void UnBind()
    {
        Updater.UpdateDic[priority].Remove(this);
    }
}

public enum EUpdatePri
{
    MainModel = 0,
    // P1 = 1,
    // P2 = 2,
}