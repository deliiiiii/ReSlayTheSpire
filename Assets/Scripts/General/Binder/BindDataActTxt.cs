using System;
using UnityEngine.UI;

public class BindDataActTxt<T> : BindDataAct<T> where T : IComparable
{
    public BindDataActTxt(Observable<T> osv, Text txt) : base(osv)
    {
        this.txt = txt;
        act = (T t) => txt.text = t.ToString();
    }
    Text txt;
    string format;
    float deltaPerSecond = float.MaxValue;
    public BindDataActTxt<T> Fluent(float deltaPerSecond)
    {
        BeforeTo();
        this.deltaPerSecond = deltaPerSecond;
        act = (_) => txt.DoFluent(osv, deltaPerSecond);
        AfterTo();
        return this;
    }

    public BindDataActTxt<T> Format(string format)
    {
        BeforeTo();
        act = (_) => txt.DoFluent(osv, deltaPerSecond, format);
        AfterTo();
        return this;
    }
}

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