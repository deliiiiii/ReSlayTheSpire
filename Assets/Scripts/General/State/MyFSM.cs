using System.Collections.Generic;
using System;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.Utilities;

public abstract class MyFSM
{
    static readonly Dictionary<Type, MyFSM> fsmDic = new();
    // onRegisterDic永远不能移除内存，是上帝规则。
    static readonly Dictionary<Type, List<Func<IMyFSMArg, IEnumerable<BindDataBase>>>> onRegisterDic = new();
    static readonly Dictionary<Type, BindDataUpdate> onUpdateDic = new();
    static readonly Dictionary<Type, List<Action<IMyFSMArg>>> onChangeDic = new();
    static readonly Dictionary<Type, IMyFSMArg> argDic = new();
    
    public static void Register<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state, TArg arg)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        if (Get<TEnum>(shouldFind: false) != null)
        {
            MyDebug.LogError("StateFactory: Acquire<" + typeof(TEnum).Name + ">Duplicated");
            return;
        }
        var type = typeof(TEnum);
        // IBL的Init，写在构造函数里了。
        argDic[type] = arg;
        fsmDic[type] = new MyFSM<TEnum>();
        // IBL的Bind
        onRegisterDic[type].ForEach(func =>func.Invoke(arg).ForEach(bindDataBase => bindDataBase.Bind()));
        // IBL的Launch
        arg.Launch();
        onUpdateDic.Add(type, Binder.FromUpdate(Get<TEnum>()!.Update, EUpdatePri.Fsm));
        onUpdateDic[type].Bind();
        onChangeDic[type].ForEach(act => act.Invoke(arg));
        EnterState(one, state);
    }
    
    public static void OnRegister<TEnum, TArg>(
        StateWrapper<TEnum, TArg> one,
        [CanBeNull] Func<TArg, IEnumerable<BindDataBase>> onRegister = null,
        [CanBeNull] Action<TArg> onChange = null)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        var type = typeof(TEnum);
        onRegisterDic.TryAdd(type, new List<Func<IMyFSMArg, IEnumerable<BindDataBase>>>());
        if(onRegister != null)
            onRegisterDic[type].Add(arg => onRegister(arg as TArg));
        onChangeDic.TryAdd(type, new List<Action<IMyFSMArg>>());
        if(onChange != null)
            onChangeDic[type].Add(arg => onChange(arg as TArg));
    }

    public static void Release<TEnum, TArg>(StateWrapper<TEnum, TArg> one)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        var fsm = Get<TEnum>();
        if (fsm == null)
            return;
        var type = typeof(TEnum);
        // 跳转到空状态
        fsm.OnDestroy();
        onUpdateDic[type].UnBind();
        onUpdateDic.Remove(typeof(TEnum));
        argDic[type].UnInit();
        onRegisterDic[type]?.ForEach(func => func.Invoke(argDic[type]).ForEach(bindDataBase => bindDataBase.UnBind()));
        fsmDic.Remove(type);
        argDic.Remove(type);
    }

    public static TArg EnterState<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        Get<TEnum>()?.ChangeState(state);
        return argDic[typeof(TEnum)] as TArg;
    }

    public static bool IsState<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
        => Get<TEnum>()?.IsOneOfState(state) ?? false;

    public static bool IsState<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state, out TArg arg)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        var ret = Get<TEnum>()?.IsOneOfState(state) ?? false;
        arg = ret ? arg = argDic[typeof(TEnum)] as TArg : null;
        return ret;
    }

    public static BindState GetBindState<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
        => new (Get<TEnum>()?.GetState(state));
    
    public static string ShowState<TEnum, TArg>(StateWrapper<TEnum, TArg> one)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
        => Get<TEnum>(shouldFind : false)?.CurStateName;
    
    [CanBeNull]
    static MyFSM<T> Get<T>(bool shouldFind = true) where T : Enum
    {
        if (fsmDic.TryGetValue(typeof(T), out var holder))
            return (MyFSM<T>)holder;
        if(shouldFind)
            MyDebug.LogError("StateFactory: Get<" + typeof(T).Name + ">Not Exist");
        return null;
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