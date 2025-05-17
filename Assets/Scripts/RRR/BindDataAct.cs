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
            //TODO 真的是每累计吗
            
            // Debug.LogWarning("1");
            // if (osv.Value is not IComparable comparableValue)
            // {
            //     Debug.LogWarning($"Observable Value {newV} is not IComparable. Immediately Triggered.");
            //     tempAct(newV);
            //     return;
            // }
            if (osv.Value.CompareTo(threshold) < 0)
                return;
            Debug.LogWarning("2");
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






