using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BindDataAct<T> where T : IComparable
{
    public BindDataAct(Observable<T> osv)
    {
        this.osv = osv;
    }
    
    protected Observable<T> osv;

    

    protected UnityAction<T> act;

    public BindDataAct<T> To(UnityAction<T> act)
    {
        BeforeTo();
        this.act = act;
        AfterTo();
        return this;
    }
    
    public BindDataActTxt<T> ToTxt(Text txt)
    {
        BeforeTo();
        var ret = new BindDataActTxt<T>(osv, txt);
        ret.AfterTo();
        return ret;
    }

    protected void BeforeTo()
    {
        osv.OnValueChangedAfter -= act;
    }
    protected void AfterTo()
    {
        osv.OnValueChangedAfter += act;
    }

    public BindDataAct<T> Immediate()
    {
        osv.Immediate();
        return this;
    }
    
    public BindDataAct<T> Culminate(float threshold)
    {
        BeforeTo();
        var tempAct = act;
        act = (T newV) =>
        {
            // Debug.LogWarning("1");
            if (osv.Value is not IComparable comparableValue)
            {
                Debug.LogWarning($"Observable Value {newV} is not IComparable. Immediately Triggered.");
                tempAct(newV);
                return;
            }
            if (comparableValue.CompareTo(threshold) < 0)
                return;
            dynamic d = osv.Value;
            osv.Value = d - threshold;
            tempAct(newV);
        };
        AfterTo();
        return this;
    }
    
    public BindDataAct<T> UnBind()
    {
        osv.OnValueChangedAfter -= act;
        return this;
    }

    public BindDataAct<T> ReBind()
    {
        osv.OnValueChangedAfter += act;
        return this;
    }
}

public class BindDataActTxt<T> : BindDataAct<T> where T : IComparable
{
    public BindDataActTxt(Observable<T> osv, Text txt) : base(osv)
    {
        this.txt = txt;
        act = (T t) => txt.text = t.ToString();
    }
    Text txt;
    float deltaPerSecond;
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



public class BindDataBtn
{
    public BindDataBtn(Button btn)
    {
        this.btn = btn;
    }
    Button btn;

    UnityAction act;
    public BindDataBtn SingleTo(UnityAction act)
    {
        UnBindAll();
        this.act = act;
        btn.onClick.AddListener(act);
        return this;
    }

    public BindDataBtn AnotherTo(UnityAction act)
    {
        this.act = act;
        btn.onClick.AddListener(act);
        return this;
    }

    void UnBindAll()
    {
        btn.onClick.RemoveAllListeners();
    }
}
