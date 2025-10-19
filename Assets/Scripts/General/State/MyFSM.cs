using System.Collections.Generic;
using System;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.Utilities;

public interface IMyFSMArg{}

public class StateWrapper<TEnum, TArg>
    where TEnum : Enum
    where TArg : class, IMyFSMArg
{
    public static StateWrapper<TEnum, TArg> One => new();
    // TEnum state;
    // TArg arg;
    // Func<TArg, IEnumerable<BindDataBase>> onRegister;
    // Action<TArg> onChange;
}

// public class SlotData2 : IMyFSMArg{}
// public class SlotData3 : IMyFSMArg{}

public abstract class MyFSM
{
    static readonly Dictionary<Type, MyFSM> fsmDic = new();
    // 这两条永远不能移除内存，是上帝规则。
    static readonly Dictionary<Type, List<Func<IMyFSMArg, IEnumerable<BindDataBase>>>> onRegisterDic = new();
    static readonly Dictionary<Type, BindDataUpdate> onUpdateDic = new();
    static readonly Dictionary<Type, List<Action<IMyFSMArg>>> onChangeDic = new();
    // static readonly Dictionary<Type, Action<IMyFSMArg>> onReleaseDic = new();
    static readonly Dictionary<Type, IMyFSMArg> argDic = new();

    // static Dictionary<EUpdatePri, SlotData2> dic;

    // public static void Register3<TEnum, TArg>(StateWrapper<TEnum, TArg> one)
    //     where TEnum : Enum
    //     where TArg : class, IMyFSMArg
    // {
    //     TEnum state = one.state;
    // }
    
    public static void Register<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state, TArg arg)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        // Register3(new GameStateWrap(state, arg))
        
        if (Get<TEnum>(shouldFind: false) != null)
        {
            MyDebug.LogError("StateFactory: Acquire<" + typeof(TEnum).Name + ">Duplicated");
            return;
        }
        argDic[typeof(TEnum)] = arg;
        fsmDic[typeof(TEnum)] = new MyFSM<TEnum>();
        onRegisterDic[typeof(TEnum)].ForEach(func =>func.Invoke(arg).ForEach(bindDataBase => bindDataBase.Bind()));
        onUpdateDic.Add(typeof(TEnum), Binder.FromUpdate(Get<TEnum>()!.Update, EUpdatePri.Fsm));
        onUpdateDic[typeof(TEnum)].Bind();
        onChangeDic[typeof(TEnum)].ForEach(act => act.Invoke(arg));
        EnterState(one, state);
    }
    
    public static void OnRegister<TEnum, TArg>(
        StateWrapper<TEnum, TArg> one,
        Func<TArg, IEnumerable<BindDataBase>> onRegister,
        Action<TArg> onChange)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        onRegisterDic.TryAdd(typeof(TEnum), new List<Func<IMyFSMArg, IEnumerable<BindDataBase>>>());
        onRegisterDic[typeof(TEnum)].Add(arg => onRegister(arg as TArg));
        onChangeDic.TryAdd(typeof(TEnum), new List<Action<IMyFSMArg>>());
        onChangeDic[typeof(TEnum)].Add(arg => onChange(arg as TArg));
    }

    public static void Release<TEnum, TArg>(StateWrapper<TEnum, TArg> one)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        var fsm = Get<TEnum>();
        if (fsm == null)
            return;
        // 跳转到空状态
        fsm.OnDestroy();
        onUpdateDic[typeof(TEnum)].UnBind();
        onUpdateDic.Remove(typeof(TEnum));
        onRegisterDic[typeof(TEnum)]?.ForEach(func => func.Invoke(argDic[typeof(TEnum)]).ForEach(bindDataBase => bindDataBase.UnBind()));
        fsmDic.Remove(typeof(TEnum));
        argDic.Remove(typeof(TEnum));
    }

    public static void EnterState<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
        => Get<TEnum>()?.ChangeState(state);
    public static bool IsState<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state, out TArg arg)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        var ret = Get<TEnum>()?.IsOneOfState(state) ?? false;
        arg = ret ? arg = argDic[typeof(TEnum)] as TArg : null;
        return ret;
    }

    public static StateHolder GetBindState<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state)
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

public class StateHolder
{
    readonly MyState state;
    public StateHolder(MyState state)
    {
        this.state = state;
    }
    
    public StateHolder OnEnter(Action act)
    {
        state.OnEnter += act;
        return this;
    }
    
    public StateHolder OnExit(Action act)
    {
        state.OnExit += act;
        return this;
    }
    
    public StateHolder OnUpdate(Action<float> act)
    {
        state.OnUpdate += act;
        return this;
    }
}
