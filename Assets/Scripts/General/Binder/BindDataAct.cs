using System;
using JetBrains.Annotations;
using UnityEngine.Events;
using UnityEngine.UI;

public class BindDataAct<T> where T : IComparable
{
    protected Observable<T> osv;
    protected UnityAction<T> act;
    UnityAction<T> latestAct;

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

    public BindDataActImg<T> ToImg(Image img, [CanBeNull] Func<float, float> func = null)
    {
        BeforeTo();
        func ??= v => v;
        var ret = new BindDataActImg<T>(osv, img, func);
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
        latestAct = act;
    }

    public BindDataAct<T> Immediate()
    {
        latestAct(osv.Value);
        return this;
    }


    float startEvery;

    public BindDataAct(Observable<T> osv)
    {
        this.osv = osv;
    }

    public BindDataAct<T> CulminateEvery(float every, int everyMaxCount = 1000)
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
                if (osv.Value.CompareTo(startEvery + every) < 0)
                    break;
                tempAct(newV);
                startEvery += every;
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






