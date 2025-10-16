using System.Collections.Generic;
using System;
using System.Linq;
using JetBrains.Annotations;

public interface IMyFSMArg{}

public class StateWrapper<TEnum, TArg>
    where TEnum : Enum
    where TArg : class, IMyFSMArg
{
    public static StateWrapper<TEnum, TArg> One => new();
}

public abstract class MyFSM
{
    static readonly Dictionary<Type, MyFSM> fsmDic = new();
    // 这两条永远不能移除内存，是上帝规则。
    static readonly Dictionary<Type, Action<IMyFSMArg>> onRegisterDic = new();
    static readonly Dictionary<Type, Action> onReleaseDic = new();

    public static void Register<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state, TArg arg)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        if (Get<TEnum>(shouldFind: false) != null)
        {
            MyDebug.LogError("StateFactory: Acquire<" + typeof(TEnum).Name + ">Duplicated");
            return;
        }
        fsmDic[typeof(TEnum)] = new MyFSM<TEnum>();
        onRegisterDic[one.GetType()]?.Invoke(arg);
        EnterState(one, state);
        GetUpdateBind<TEnum>().Bind();
    }
    
    public static void OnRegister<TEnum, TArg>(StateWrapper<TEnum, TArg> one, Action<TArg> onRegister)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        onRegisterDic.TryAdd(one.GetType(), null);
        onRegisterDic[one.GetType()] += v => onRegister(v as TArg);
    }

    public static void Release<TEnum, TArg>(StateWrapper<TEnum, TArg> one)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        var fsm = Get<TEnum>();
        if (fsm == null)
            return;
        // 跳转到空状态
        GetUpdateBind<TEnum>().UnBind();
        fsm.OnDestroy();
        onReleaseDic[one.GetType()]?.Invoke();
        fsmDic.Remove(typeof(TEnum));
    }
    public static void OnRelease<TEnum, TArg>(StateWrapper<TEnum, TArg> one, Action onRelease) 
        where TEnum : Enum
        where TArg : class, IMyFSMArg
    {
        onReleaseDic.TryAdd(one.GetType(), null);
        onReleaseDic[one.GetType()] += onRelease;
    }

    public static void EnterState<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
        => Get<TEnum>()?.ChangeState(state);
    public static bool IsState<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
        => Get<TEnum>()?.IsOneOfState(state) ?? false;
    public static BindDataState GetBindState<TEnum, TArg>(StateWrapper<TEnum, TArg> one, TEnum state)
        where TEnum : Enum
        where TArg : class, IMyFSMArg
        => Binder.From(Get<TEnum>()?.GetState(state));
    
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
