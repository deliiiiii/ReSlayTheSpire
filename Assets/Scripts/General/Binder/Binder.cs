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

    public static SortedDictionary<EUpdatePri, HashSet<BindDataUpdate>> UpdateDic = new();
    public static BindDataUpdate Update(Action act, EUpdatePri priority)
    {
        var ret = new BindDataUpdate(act, priority);
        if(!UpdateDic.ContainsKey(priority))
            UpdateDic.Add(priority, new HashSet<BindDataUpdate>());
        UpdateDic[priority].Add(ret);
        return ret;
    }
}

public class BindDataUpdate
{
    public Action Act;
    readonly EUpdatePri priority;

    public BindDataUpdate(Action act, EUpdatePri priority)
    {
        this.Act = act;
        this.priority = priority;
    }

    public void UnBind()
    {
        Binder.UpdateDic[priority].Remove(this);
    }
}

public enum EUpdatePri
{
    Default = 0,
    P1 = 1,
    P2 = 2,
}