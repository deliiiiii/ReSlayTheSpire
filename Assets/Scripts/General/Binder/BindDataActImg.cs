﻿using System;
using UnityEngine.UI;

public class BindDataActImg<T> : BindDataAct<T> where T : IComparable
{
    public BindDataActImg(Observable<T> osv, Image img, Func<T, float> func) : base(osv)
    {
        this.img = img;
        this.func = func;
        act = (_) => img.fillAmount = (dynamic)func(osv);
    }
    Image img;
    string format;
    Func<T, float> func;
    float deltaPerSecond = float.MaxValue;
    public BindDataActImg<T> Fluent(float deltaPerSecond)
    {
        BeforeTo();
        this.deltaPerSecond = deltaPerSecond;
        act = (_) => img.DoFluent(func(osv), deltaPerSecond);
        AfterTo();
        return this;
    }
}