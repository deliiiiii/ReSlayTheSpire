using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.Utilities;

public interface IFSM<in TEnum>
    where TEnum : struct, Enum
{
    MyStateForView GetState(TEnum e);
    void EnterState(TEnum e);
    bool IsState(TEnum e);
}

public abstract class FSMArgForView<TArg, TEnum> : IFSM<TEnum>
    where TArg : FSMArgForView<TArg, TEnum>
    where TEnum : struct, Enum
{
    // bindAlwaysDic包括了State的OnEnter、OnExit，和Data的事件绑定回调，如data.OnXXXEvent += () => {}
    [JsonIgnore] protected static readonly List<Action<TArg, IFSM<TEnum>>> AlwaysBindList = [];
    // unbindDic包括BindDataBase的子类的Bind/UnBind，如进入状态时绑定UI按钮且退出状态解绑
    [JsonIgnore] protected static readonly List<Func<TArg, IFSM<TEnum>, IEnumerable<BindDataBase>>> 
        CanUnbindList = [];
    
    public static void OnRegister(
        Action<TArg, IFSM<TEnum>>? alwaysBind = null,
        Func<TArg, IFSM<TEnum>, IEnumerable<BindDataBase>>? canUnbind = null)
    {
        if(canUnbind != null)
            CanUnbindList.Add(canUnbind);
        if(alwaysBind != null)
            AlwaysBindList.Add(alwaysBind.Invoke);
    }
    
    
    protected TEnum CurState;
    [JsonIgnore] protected MyState? CurStateClass;
    [JsonIgnore] protected readonly Dictionary<TEnum, MyState> StateDic = [];

    public MyStateForView GetState(TEnum e) => GetStateInternal(e);
    protected MyState GetStateInternal(TEnum e)
    {
        if (StateDic.TryGetValue(e, out var value))
            return value;
        MyState state = new();
        StateDic.Add(e, state);
        return state;
    }
    
    public void EnterState(TEnum e)
    {
        if (CurStateClass == null)
        {
            MyDebug.LogError("FSM Not Launched");
            return;
        }
        var newStateClass = GetStateInternal(e);
        if (newStateClass == CurStateClass)
            return;
        CurStateClass.Exit();
        CurStateClass = newStateClass;
        CurState = e;
        CurStateClass.Enter();
    }
    
    public bool IsOneOfState(params TEnum[] enums) => enums.Contains(CurState);
    public bool IsState(TEnum e) => IsOneOfState(e);
}


public abstract class FSMArg<TArg, TEnum> : FSMArgForView<TArg, TEnum>
    where TArg : FSMArg<TArg, TEnum>
    where TEnum : struct, Enum
{
    protected FSMArg()
    {
        selfTick = Binder.FromTick(Tick, EUpdatePri.Fsm);
    }
    protected TArg Arg => (this as TArg)!;
    bool isLaunched;
    [JsonIgnore] readonly BindDataUpdate selfTick;
    [JsonIgnore] readonly List<BindDataBase> unbindableInstances = [];
    public void Launch(TEnum startState)
    {
        if (isLaunched)
        {
            MyDebug.LogError($"Launch FSM: {GetType().Name} Duplicated");
            return;
        }
        // 【0】添加FSM
        isLaunched = true;
        // 【1】IBL的Init, 构造函数
        // 【2】IBL的Bind
        Bind();
        unbindableInstances.Clear();
        CanUnbindList.ForEach(func =>
        {
            func.Invoke(Arg, this).ForEach(bdb =>
            {
                bdb.Bind();
                unbindableInstances.Add(bdb);
            });
        });
        AlwaysBindList.ForEach(bindAlwaysAct => bindAlwaysAct.Invoke(Arg, this));
        selfTick.Bind();
        // 【3】IBL的Launch
        Launch();
        // 【4】进入初始状态
        CurStateClass = GetState(startState);
        CurState = startState;
        CurStateClass.Enter();
    }

    public void Release()
    {
        if (!isLaunched)
        {
            MyDebug.LogError($"Release FSM: {GetType().Name} Not Exist");
            return;
        }
        // 【4】跳转到空状态，并清空所有状态类
        CurStateClass?.Exit();
        StateDic.Clear();
        CurStateClass = null;
        // 【3】Launch的反向
        UnInit();
        // 【2】Bind的反向
        selfTick.UnBind();
        unbindableInstances.ForEach(instance => instance.UnBind());
        unbindableInstances.Clear();
        // 【1】Init的反向, 无需析构函数
        // 【0】移除FSM
        isLaunched = false;
    }
    
    public string CurStateName => CurState.ToString();

    protected new MyState GetState(TEnum e) => GetStateInternal(e);

    protected abstract void Bind();
    protected abstract void Launch();
    protected abstract void UnInit();

    protected virtual void Tick(float dt)
    {
        CurStateClass?.Update(dt);
    }
}

public abstract class FSMArg<TArg, TEnum, TParentFSMArg>(TParentFSMArg parent)
    : FSMArg<TArg, TEnum>
    where TArg : FSMArg<TArg, TEnum, TParentFSMArg>
    where TEnum : struct, Enum
{
    public readonly TParentFSMArg Parent = parent;
}
