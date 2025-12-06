using System;
using System.Diagnostics.CodeAnalysis;

namespace RSTS;

public abstract class FSM2<TOuter, TThis, TBaseState>
    where TThis : FSM2<TOuter, TThis, TBaseState>
    where TBaseState : StateBase<TThis>
{
    public static event Action<TBaseState>? OnStateEnter;
    public static event Action<TBaseState>? OnStateExit;
    // unbindDic包括BindDataBase的子类的Bind/UnBind，如进入状态时绑定UI按钮且退出状态解绑
    // [JsonIgnore] static readonly List<Func<TBaseState, IEnumerable<BindDataBase>>> canUnbindList = [];
    // public static TBaseContext Create<TSubState>() 
    //     where TSubState : TBaseState
    // {
    //     var ret = Activator.CreateInstance<TBaseContext>();
    //     ret.selfTick = Binder.FromTick(ret.Tick, EUpdatePri.Fsm);
    //     ret.Launch<TSubState>();
    //     return ret;
    // }
    // [JsonIgnore] BindDataUpdate selfTick;
    public required TOuter Outer;
    bool isLaunched;
    TBaseState? curState;
    // [JsonIgnore] readonly List<BindDataBase> unbindableInstances = [];

    public void Launch<TSubState>() where TSubState : TBaseState
    {
        isLaunched = true;
        EnterState<TSubState>();
    }
    // public abstract void Init();
    // /// 初始化Context
    // public abstract void Bind();
    // public abstract void UnInit();

    // protected virtual void Tick(float dt)
    // {
    // curState?.Update(dt);
    // }
    
    public void EnterState<TSubState>() where TSubState : TBaseState
    {
        if (!isLaunched)
        {
            MyDebug.LogError($"{GetType().Name} Enter State But NOT Launched");
            return;
        }
        if(curState != null)
            OnStateExit?.Invoke(curState);
        curState?.OnExit();
        curState = (TSubState)Activator.CreateInstance(typeof(TSubState), this);
        curState.FSM = (TThis)this;
        OnStateEnter?.Invoke(curState);
    }
    public bool IsState<TSubState>([NotNullWhen(true)] out TSubState subState)
        where TSubState : StateBase<TThis>
    {
        subState = null!;
        if (curState is TSubState state)
        {
            subState = state;
            return true;
        }
        return false;
    }
}


public abstract class StateBase<TFSM>
    // : FSM2<object, TContext, TThis>, IState<TThis> 
    // where TContext : FSM2<object, TContext, TThis> 
    // where TThis : class, IState<TContext>
{
    public required TFSM FSM;
    public virtual void OnExit() { }
    public virtual void OnUpdate(float dt) {}
}
