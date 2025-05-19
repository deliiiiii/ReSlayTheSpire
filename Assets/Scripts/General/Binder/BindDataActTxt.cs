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