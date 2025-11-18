using System.Collections.Generic;
using System;
using System.Linq;
using Sirenix.Utilities;

public class MyFSMForView<TEnum, TArg>
    where TEnum : struct, Enum
    where TArg : class
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

    protected MyState GetStateSub(TEnum e) => (GetState(e) as MyState)!;
}

public class MyFSMForView<TForData, TEnum, TArg> : MyFSMForView<TEnum, TArg>
    where TForData : MyFSMForView<TForData, TEnum, TArg>, new()
    where TEnum : struct, Enum
    where TArg : class, IMyFSMArg<TForData>
{
    protected static readonly TForData One = new ();
    public static void OnRegister(
        Action<MyFSMForView<TEnum, TArg>>? alwaysBind = null,
        Func<MyFSMForView<TEnum, TArg>, IEnumerable<BindDataBase>>? canUnbind = null,
        Action<float, TArg>? tick = null)
    {
        if(canUnbind != null)
            One.CanUnBind.Add(canUnbind);
        if(alwaysBind != null)
            One.BindAlways.Add(alwaysBind.Invoke);
        if(tick != null)
            One.Tick.Add(Binder.FromTick(dt => tick(dt, One.Arg), EUpdatePri.Fsm));
    }
    
    // bindAlwaysDic包括了State的OnEnter、OnExit，和Data的事件绑定回调，如data.OnXXXEvent += () => {}
    internal readonly List<Action<MyFSMForView<TEnum, TArg>>> BindAlways = [];
    // unbindDic包括BindDataBase的子类的Bind/UnBind，如进入状态时绑定UI按钮且退出状态解绑
    internal readonly List<Func<MyFSMForView<TEnum, TArg>, IEnumerable<BindDataBase>>> CanUnBind = [];
    // tickDic包括自己绑定的与Data有关的Tick委托（类型Action<float, IMyFSMArg>）
    internal readonly List<BindDataUpdate> Tick = [];
}


[Serializable]
public class MyFSMForData<TForData, TEnum, TArg> : MyFSMForView<TForData, TEnum, TArg>
    where TForData : MyFSMForData<TForData, TEnum, TArg>, new()
    where TEnum : struct, Enum
    where TArg : class, IMyFSMArg<TForData>
{
    public static void EnterStateStatic(TEnum state) => One.EnterState(state);
    public static bool IsStateStatic(TEnum state) => One.IsOneOfState(state);
    public static bool IsStateStatic(TEnum state, out MyFSMForView<TEnum, TArg> fsm)
    {
        var ret = One.IsOneOfState(state);
        fsm = One;
        return ret;
    }
    public static string CurStateNameStatic => One.CurState?.ToString() ?? "Null";
    
    
    bool isRegistered;
    BindDataUpdate selfTick = null!;
    readonly List<BindDataBase> unbindableInstances = [];
    
    public static void Register(TEnum state, TArg arg)
    {
        if (One.isRegistered)
        {
            MyDebug.LogError($"Register FSM: {typeof(TForData).Name} Duplicated");
            return;
        }
        // 【0】添加FSM
        One.isRegistered = true;
        // 【1】IBL的Init，写在构造函数里了。
        One.Arg = arg;
        // 【1.1】绑定自身Tick
        One.selfTick = Binder.FromTick(One.Update, EUpdatePri.Fsm);
        // 【2】IBL的Bind
        One.Arg.Bind(One);
        One.unbindableInstances.Clear();
        One.unbindableInstances.Clear();
        One.CanUnBind.ForEach(func =>
        {
            func.Invoke(One).ForEach(bdb =>
            {
                bdb.Bind();
                One.unbindableInstances.Add(bdb);
            });
        });
        One.BindAlways.ForEach(bindAlwaysAct => bindAlwaysAct.Invoke(One));
        One.Tick.ForEach(bindDataUpdate => bindDataUpdate.Bind());
        // 【3】IBL的Launch
        One.Arg.Launch();
        // 【4】进入初始状态
        One.Launch(state);
    }

    public static void Release()
    {
        if (!One.isRegistered)
        {
            MyDebug.LogError($"Release FSM: {typeof(TForData).Name} Not Exist");
            return;
        }
        // 【4】跳转到空状态，并清空所有状态类
        One.OnDestroy();
        // 【3】Launch的反向
        One.Arg.UnInit();
        // 【2】Bind的反向
        One.Tick.ForEach(bindDataUpdate => bindDataUpdate.UnBind());
        One.unbindableInstances.ForEach(instance => instance.UnBind());
        One.unbindableInstances.Clear();
        // 【1.1】自身Tick的反向。
        One.selfTick.UnBind();
        // 【1】构造函数不用反向。
        One.Arg = null!;
        // 【0】移除Wrap
        One.isRegistered = false;
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
    protected bool IsOneOfState(params Enum[] enums) => enums.Contains(CurState);
}