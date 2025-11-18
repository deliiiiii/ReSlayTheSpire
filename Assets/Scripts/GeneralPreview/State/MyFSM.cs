using System.Collections.Generic;
using System;
using System.Linq;
using Sirenix.Utilities;

public abstract class MyFSMForView<TEnum, TArg>
    where TEnum : struct, Enum
    where TArg : class, IMyFSMArg<TEnum>
{
    public TArg Arg = null!;
    protected MyState? CurStateClass;
    protected Enum? CurState;
    protected readonly Dictionary<TEnum, MyState> StateDic = [];
    
    public MyStateForView GetState(TEnum e)
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
        var newStateClass = GetStateSub(e);
        if (newStateClass == CurStateClass)
            return;
        CurStateClass.Exit();
        CurStateClass = newStateClass;
        CurState = e;
        CurStateClass.Enter();
    }
    public bool IsOneOfState(params Enum[] enums) => enums.Contains(CurState);
    public bool IsState(TEnum e) => IsOneOfState(e);
    public string CurStateName => CurState?.ToString() ?? "Null";

    protected MyState GetStateSub(TEnum e) => (GetState(e) as MyState)!;
    
    
    public void OnRegister(
        Action<MyFSMForView<TEnum, TArg>>? alwaysBind = null,
        Func<MyFSMForView<TEnum, TArg>, IEnumerable<BindDataBase>>? canUnbind = null,
        Action<float, TArg>? tick = null)
    {
        if(canUnbind != null)
            CanUnBind.Add(canUnbind);
        if(alwaysBind != null)
            BindAlways.Add(alwaysBind.Invoke);
        if(tick != null)
            Tick.Add(Binder.FromTick(dt => tick(dt, Arg), EUpdatePri.Fsm));
    }
    
    // bindAlwaysDic包括了State的OnEnter、OnExit，和Data的事件绑定回调，如data.OnXXXEvent += () => {}
    internal readonly List<Action<MyFSMForView<TEnum, TArg>>> BindAlways = [];
    // unbindDic包括BindDataBase的子类的Bind/UnBind，如进入状态时绑定UI按钮且退出状态解绑
    internal readonly List<Func<MyFSMForView<TEnum, TArg>, IEnumerable<BindDataBase>>> CanUnBind = [];
    // tickDic包括自己绑定的与Data有关的Tick委托（类型Action<float, IMyFSMArg>）
    internal readonly List<BindDataUpdate> Tick = [];
}

[Serializable]
public abstract class MyFSMForData<TEnum, TArg> : MyFSMForView<TEnum, TArg>
    where TEnum : struct, Enum
    where TArg : class, IMyFSMArg<TEnum>
{
    bool isRegistered;
    BindDataUpdate selfTick = null!;
    readonly List<BindDataBase> unbindableInstances = [];
    
    public void Register(TEnum state, TArg arg)
    {
        if (isRegistered)
        {
            MyDebug.LogError($"Register FSM: {GetType().Name} Duplicated");
            return;
        }
        // 【0】添加FSM
        isRegistered = true;
        // 【1】IBL的Init，写在构造函数里了。
        Arg = arg;
        // 【1.1】绑定自身Tick
        selfTick = Binder.FromTick(Update, EUpdatePri.Fsm);
        // 【2】IBL的Bind
        Arg.Bind(GetStateSub);
        unbindableInstances.Clear();
        unbindableInstances.Clear();
        CanUnBind.ForEach(func =>
        {
            func.Invoke(this).ForEach(bdb =>
            {
                bdb.Bind();
                unbindableInstances.Add(bdb);
            });
        });
        BindAlways.ForEach(bindAlwaysAct => bindAlwaysAct.Invoke(this));
        Tick.ForEach(bindDataUpdate => bindDataUpdate.Bind());
        // 【3】IBL的Launch
        Arg.Launch();
        // 【4】进入初始状态
        Launch(state);
    }

    public void Release()
    {
        if (!isRegistered)
        {
            MyDebug.LogError($"Release FSM: {GetType().Name} Not Exist");
            return;
        }
        // 【4】跳转到空状态，并清空所有状态类
        OnDestroy();
        // 【3】Launch的反向
        Arg.UnInit();
        // 【2】Bind的反向
        Tick.ForEach(bindDataUpdate => bindDataUpdate.UnBind());
        unbindableInstances.ForEach(instance => instance.UnBind());
        unbindableInstances.Clear();
        // 【1.1】自身Tick的反向。
        selfTick.UnBind();
        // 【1】构造函数不用反向。
        Arg = null!;
        // 【0】移除Wrap
        isRegistered = false;
    }
    
    
    public new MyState GetState(TEnum e) => GetStateSub(e);
    
    internal void Launch(TEnum startState)
    {
        // foreach (var e in Enum.GetValues(typeof(TEnum))) 
        //     StateDic.Add((TEnum)e, new MyState());
        CurStateClass = GetStateSub(startState);
        CurState = startState;
        CurStateClass.Enter();
    }
    void Update(float dt) => CurStateClass?.Update(dt);
    internal void OnDestroy()
    {
        CurStateClass?.Exit();
        StateDic.Clear();
        
        CurState = null;
        CurStateClass = null;
    }
}