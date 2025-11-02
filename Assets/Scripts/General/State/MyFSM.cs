using System.Collections.Generic;
using System;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.Utilities;

public abstract class MyFSM
{
    static readonly Dictionary<Type, MyFSM> fsmDic = new();
    // bindAlwaysDic包括了State的OnEnter、OnExit，和Data的事件绑定回调，如data.OnXXXEvent += () => {}
    static readonly Dictionary<Type, List<Action<IMyFSMArg>>> bindAlwaysDic = new();
    // unbindDic包括BindDataBase的子类的Bind/UnBind，如进入状态时绑定UI按钮且退出状态解绑
    static readonly Dictionary<Type, List<Func<IMyFSMArg, IEnumerable<BindDataBase>>>> unbindDic = new();
    // tickDic包括状态机的Update调用的OnUpdate，和自己绑定的与Data有关的Tick委托（类型Action<float, IMyFSMArg>）
    static readonly Dictionary<Type, List<BindDataUpdate>> tickDic = new();
    // 只有Register了以后，才会加入selfTickDic
    static readonly Dictionary<Type, BindDataUpdate> selfTickDic = new();
    // 只有Register了以后，才会加入argDic
    static readonly Dictionary<Type, IMyFSMArg> argDic = new();
    
    public static void Register<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state, Func<MyFSM<TEnum>, TArg> argCtor)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        var fsm = Get<TEnum>(shouldFind: false);
        if (fsm != null)
        {
            MyDebug.LogError("StateFactory: Acquire<" + typeof(TEnum).Name + ">Duplicated");
            return;
        }
        var type = typeof(TEnum);
        // IBL的Init，写在构造函数里了。
        fsmDic[type] = fsm = new MyFSM<TEnum>();
        var arg = argCtor(fsm);
        argDic[type] = arg;
        // IBL的Bind
        if (unbindDic.TryGetValue(type, out var unbindFuncList)) 
            unbindFuncList?.ForEach(func =>func.Invoke(arg).ForEach(bindDataBase => bindDataBase.Bind()));
        if (bindAlwaysDic.TryGetValue(type, out var bindAlwaysActList)) 
            bindAlwaysActList.ForEach(bindAlwaysAct => bindAlwaysAct.Invoke(arg));
        // IBL的Launch
        arg.Launch();
        if(tickDic.TryGetValue(type, out var tickActList))
            tickActList.ForEach(bindDataUpdate => bindDataUpdate.Bind());
        selfTickDic.Add(type, Binder.FromTick(dt => fsm.Update(dt), EUpdatePri.Fsm));
        selfTickDic[type].Bind();
        EnterState(one, state);
    }
    
    public static void OnRegister<TEnum, TArg>(
        StateWrapper<TEnum, TArg> one,
        [CanBeNull] Action<MyFSM<TEnum>, TArg> alwaysBind = null,
        [CanBeNull] Func<TArg, IEnumerable<BindDataBase>> canUnbind = null,
        [CanBeNull] Action<float, TArg> tick = null)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        var type = typeof(TEnum);
        unbindDic.TryAdd(type, new List<Func<IMyFSMArg, IEnumerable<BindDataBase>>>());
        if(canUnbind != null)
            unbindDic[type].Add(arg => canUnbind(arg as TArg));
        bindAlwaysDic.TryAdd(type, new List<Action<IMyFSMArg>>());
        if(alwaysBind != null)
            bindAlwaysDic[type].Add(arg => alwaysBind.Invoke(Get<TEnum>(), arg as TArg));
        tickDic.TryAdd(type, new List<BindDataUpdate>());
        if(tick != null)
            tickDic[type].Add(Binder.FromTick(dt => tick(dt, argDic[type] as TArg), EUpdatePri.Fsm));
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
        selfTickDic[type].UnBind();
        selfTickDic.Remove(type);
        tickDic[type].ForEach(bindDataUpdate => bindDataUpdate.UnBind());
        argDic[type].UnInit();
        unbindDic[type]?.ForEach(func => func.Invoke(argDic[type]).ForEach(bindDataBase => bindDataBase.UnBind()));
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