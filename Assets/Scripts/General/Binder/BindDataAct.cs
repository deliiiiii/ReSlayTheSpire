using System;
using JetBrains.Annotations;
using UnityEngine.Events;
using UnityEngine.UI;

public class BindDataAct<T> : BindDataBase
{
    protected Observable<T> osv;
    protected UnityAction<T> act;
    bool isImmediate;

    public BindDataAct<T> To(UnityAction<T> fAct)
    {
        osv.OnValueChangedAfter -= fAct;
        act = fAct;
        return this;
    }
    
    // public BindDataActTxt<T> ToTxt(Text txt)
    // {
    //     BeforeTo();
    //     var ret = new BindDataActTxt<T>(osv, txt);
    //     ret.AfterTo();
    //     return ret;
    // }
    //
    // public BindDataActImg<T> ToImg(Image img, [CanBeNull] Func<float, float> func = null)
    // {
    //     BeforeTo();
    //     func ??= v => v;
    //     var ret = new BindDataActImg<T>(osv, img, func);
    //     ret.AfterTo();
    //     return ret;
    // }

    public void Immediate()
    {
        isImmediate = true;
    }
    
    public BindDataAct(Observable<T> osv)
    {
        this.osv = osv;
    }

    public override void Bind()
    {
        osv.OnValueChangedAfter += act;
        if (isImmediate)
        {
            act(osv.Value);
        }
    }

    public override void UnBind()
    {
        osv.OnValueChangedAfter -= act;
    }
}






