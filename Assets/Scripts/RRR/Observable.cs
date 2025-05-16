using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class Observable<T> where T: IComparable
{
    [SerializeField]
    T _value;
    public T Value
    {
        get => _value;
        set
        {
            if (Equals(value, _value))
            {
                return;
            }
            // OnValueChangedBefore?.Invoke(_value);
            _value = value;
            OnValueChangedAfter?.Invoke(_value);
        }
    }
    // public event UnityAction<T> OnValueChangedBefore;
    public event UnityAction<T> OnValueChangedAfter;

    public void Immediate()
    {
        OnValueChangedAfter?.Invoke(_value);
    }
    public Observable(T initValue)
    {
        _value = initValue;
    }
    // public Observable(T initValue, UnityAction<T> before, UnityAction<T> after)
    public Observable(T initValue, UnityAction<T> after)
    {
        _value = initValue;
        // OnValueChangedBefore += before;
        OnValueChangedAfter += after;
    }
    public static implicit operator T(Observable<T> v)
    {
        return v.Value;
    }
    
    public static implicit operator float(Observable<T> v)
    {
        return v.Value switch
        {
            int i => i,
            float f => f,
            double d => (float)d,
            char c => c,
            _ => 0f
        };
    }
}



public static class Binder
{
    public static BindDataAct<T> From<T>(Observable<T> osv) where T : IComparable
    {
        return new BindDataAct<T>(osv);
    }
    public static BindDataBtn From(Button btn)
    {
        return new BindDataBtn(btn);
    }
    public static BindDataBtn From(GameObject pnl)
    {
        return From(pnl.GetComponent<Button>());
    }
}
