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

    public void Immediate()
    {
        osv.Immediate();
    }


    T startEvery;
    public BindDataAct<T> CulminateEvery(T every, int everyMaxCount = 1000)
    {
        BeforeTo();
        var tempAct = act;
        startEvery = osv;
        act = (newV) =>
        {
            int tempCount = 0;
            while (true)
            {
                if (tempCount >= everyMaxCount)
                    break;
                if (osv.Value.CompareTo((dynamic)startEvery + (dynamic)every) < 0)
                    break;
                tempAct(newV);
                startEvery += (dynamic)every;
                tempCount++;
            }
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






