using System;
using System.Collections.Generic;

public static class StateFactory
{
    static readonly Dictionary<Type, object> fsmHolders = [];
    public static MyFSM<T> Register<T>() where T : Enum
    {
        if (fsmHolders.TryGetValue(typeof(T), out var holder))
        {
            MyDebug.LogError("StateFactory: Acquire<" + typeof(T).Name + ">Duplicated");
            return (MyFSM<T>)holder;
        }
        var added = new MyFSM<T>();
        fsmHolders[typeof(T)] = added;
        return added;
    }
    
    public static void Release<T>() where T : Enum
    {
        fsmHolders.Remove(typeof(T));
    }
    public static void EnterState<T>(T state) where T : Enum
        => Get<T>().ChangeState(state);
    public static bool IsState<T>(T state) where T : Enum
        => Get<T>().IsState(state);
    public static BindDataState GetBindState<T>(T state) where T : Enum
        => Binder.From(Get<T>().GetState(state));
    
    public static IEnumerable<BindDataBase> GetAllBinders<T>() where T : Enum
    {
        yield return Binder.From(Get<T>().Update, EUpdatePri.Fsm);
    }
    
    static MyFSM<T> Get<T>() where T : Enum
    {
        if (fsmHolders.TryGetValue(typeof(T), out var holder))
        {
            return (MyFSM<T>)holder;
        }
        MyDebug.LogError("StateFactory: Get<" + typeof(T).Name + ">Not Exist");
        return new MyFSM<T>();
    }
}