using System.Collections.Generic;
using System;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.Utilities;

public abstract class MyFSM
{
    static readonly Dictionary<Type, MyFSM> fsmDic = new();
    // 这两条永远不能移除内存，是上帝规则。
    static readonly Dictionary<Type, Action> onRegisterDic = new();
    static readonly Dictionary<Type, Action> onReleaseDic = new();
    public static void Register<T>(T initState) where T : Enum
    {
        if (Get<T>(shouldFind: false) != null)
        {
            MyDebug.LogError("StateFactory: Acquire<" + typeof(T).Name + ">Duplicated");
            return;
        }
        fsmDic[typeof(T)] = new MyFSM<T>();
        onRegisterDic[typeof(T)]?.Invoke();
        EnterState(initState);
        GetUpdateBind<T>().Bind();
    }
    public static void OnRegister<T>(Action onRegister) where T : Enum
    {
        onRegisterDic.TryAdd(typeof(T), null);
        onRegisterDic[typeof(T)] += onRegister;
    }
    public static void Release<T>() where T : Enum
    {
        var fsm = Get<T>();
        if (fsm == null)
            return;
        // 跳转到空状态
        GetUpdateBind<T>().UnBind();
        fsm.OnDestroy();
        onReleaseDic[typeof(T)]?.Invoke();
        fsmDic.Remove(typeof(T));
    }
    public static void OnRelease<T>(Action onRelease) where T : Enum
    {
        onReleaseDic.TryAdd(typeof(T), null);
        onReleaseDic[typeof(T)] += onRelease;
    }

    public static void EnterState<T>(T state) where T : Enum
        => Get<T>()?.ChangeState(state);
    public static bool IsState<T>(T state) where T : Enum
        => Get<T>()?.IsOneOfState(state) ?? false;
    public static BindDataState GetBindState<T>(T state) where T : Enum
        => Binder.From(Get<T>()?.GetState(state));
    
    public static string ShowState<T>() where T : Enum
        => Get<T>(shouldFind : false)?.CurStateName;
    
    [CanBeNull]
    static MyFSM<T> Get<T>(bool shouldFind = true) where T : Enum
    {
        if (fsmDic.TryGetValue(typeof(T), out var holder))
            return (MyFSM<T>)holder;
        if(shouldFind)
            MyDebug.LogError("StateFactory: Get<" + typeof(T).Name + ">Not Exist");
        return null;
    }
    
    static BindDataBase GetUpdateBind<T>() where T : Enum
    {
        var fsm = Get<T>();
        if (fsm == null)
            return null;
        return Binder.FromUpdate(fsm.Update, EUpdatePri.Fsm);
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

    public void OnDestroy()
    {
        curStateClass.Exit();
        curState = null;
        curStateClass = null;
    }
}
