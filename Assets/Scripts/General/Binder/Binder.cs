﻿using System;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Binder
{
    public static BindDataObs<T> FromObs<T>(Observable<T> osv) 
    {
        return new BindDataObs<T>(osv);
    }

    public static BindDataEvent FromBtn(Button btn)
        => new(btn.onClick);
    
    // public static BindDataEvent From(UnityEvent evt)
    //     => new(evt);

    //
    // public static BindDataEvent<T> From<T>(UnityEvent<T> evt)
    // {
    //     return new BindDataEvent<T>(evt);
    // }
    
    public static BindDataUpdate FromTick(Action<float> act, EUpdatePri priority = EUpdatePri.Default)
    {
        return new BindDataUpdate(act, priority);
    }
    public static BindDataUpdate FromTick<TEnum>(Action<float> act, TEnum priority)
    {
        return new BindDataUpdate(act, (EUpdatePri)Convert.ToInt32(priority));
    }
}

