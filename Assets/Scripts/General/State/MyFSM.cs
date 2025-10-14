using System.Collections.Generic;
using System;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public abstract class MyFSM
{
    static readonly Dictionary<Type, object> fsmDic = new();
    public static void Register<T>() where T : Enum
    {
        if (fsmDic.ContainsKey(typeof(T)))
        {
            MyDebug.LogError("StateFactory: Acquire<" + typeof(T).Name + ">Duplicated");
            return;
        }
        var added = new MyFSM<T>();
        fsmDic[typeof(T)] = added;
    }
    
    public static void Release<T>() where T : Enum
    {
        GetAllBinders<T>().ForEach(b => b.UnBind());
        fsmDic.Remove(typeof(T));
    }

    public static void EnterState<T>(T state) where T : Enum
        => Get<T>().ChangeState(state);
    public static bool IsState<T>(T state) where T : Enum
        => Get<T>().IsOneOfState(state);
    public static BindDataState GetBindState<T>(T state) where T : Enum
        => Binder.From(Get<T>().GetState(state));
    
    public static IEnumerable<BindDataBase> GetAllBinders<T>() where T : Enum
    {
        yield return Binder.From(Get<T>().Update, EUpdatePri.Fsm);
    }
    
    public static string ShowState<T>() where T : Enum
        => Get<T>().CurStateName;
    
    static MyFSM<T> Get<T>() where T : Enum
    {;
        if (fsmDic.TryGetValue(typeof(T), out var holder))
            return (MyFSM<T>)holder;
        MyDebug.LogError("StateFactory: Get<" + typeof(T).Name + ">Not Exist");
        return new MyFSM<T>();
    }
}

[Serializable]
public class MyFSM<TEnum> : MyFSM 
    where TEnum : Enum
{ 
    public MyFSM()
    {
        stateDic = new Dictionary<TEnum, MyState>();
        foreach (var e in Enum.GetValues(typeof(TEnum))) 
            stateDic.Add((TEnum)e, new MyState());
    }

    public string CurStateName => curState?.ToString() ?? "Null";
    Dictionary<TEnum, MyState> stateDic;
    MyState curStateClass;
    Enum curState;
    
    public MyState GetState(TEnum e)
    {
        if (e == null)
            return null;
        if (stateDic.TryGetValue(e, out var value))
            return value;
        MyState state = new();
        stateDic.Add(e, state);
        return state;
    }
    
    public void Update(float dt) => curStateClass?.Update(dt);

    public void ChangeState(TEnum e)
    {
        if (curStateClass == null)
        {
            Launch(e);
            return;
        }
        var newStateClass = GetState(e);
        if (newStateClass == curStateClass)
            return;
        curStateClass.Exit();
        curStateClass = newStateClass;
        curState = e;
        curStateClass.Enter();
    }

    public bool IsOneOfState(params Enum[] enums) => enums.Contains(curState);
    void Launch(TEnum startState)
    {
        curStateClass = GetState(startState);
        curState = startState;
        curStateClass.Enter();
    }
}
