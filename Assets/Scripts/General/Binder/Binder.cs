using System;
using System.Collections.Generic;
using General.Binder;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public static class Binder
{
    public static BindDataAct<T> From<T>(Observable<T> osv) 
    {
        return new BindDataAct<T>(osv);
    }
    public static BindDataActComparable<T> FromComparable<T>(Observable<T> osv) where T : IComparable
    {
        return new BindDataActComparable<T>(osv);
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

    public static BindDataUpdate Update(Action<float> act, EUpdatePri priority = EUpdatePri.Default)
    {
        var ret = new BindDataUpdate(act, priority);
        Updater.UpdateDic.TryAdd(ret.Priority, new HashSet<BindDataUpdate>());
        Updater.UpdateDic[ret.Priority].Add(ret);
        return ret;
    }

    public static BindDataUpdate<TEnum> Update<TEnum>(Action<float> act, TEnum priority)
        where TEnum : Enum
    {
        var ret = new BindDataUpdate<TEnum>(act, priority);
        Updater.UpdateDic.TryAdd(ret.Priority, new HashSet<BindDataUpdate>());
        Updater.UpdateDic[ret.Priority].Add(ret);
        return ret;
    }
    
    public static void RemoveUpdate([CanBeNull] BindDataUpdate bindDataUpdate)
    {
        if (bindDataUpdate == null)
            return;
        if (Updater.UpdateDic.TryGetValue(bindDataUpdate.Priority, out var set))
        {
            set.Remove(bindDataUpdate);
        }
    }
}

