using System;
using System.Diagnostics.CodeAnalysis;

namespace RSTS;
public abstract class FSM2<TContext, TBaseState>(TContext context)
    where TBaseState : FSM2<TContext, TBaseState>.StateBase
{
    public static event Action<TBaseState>? OnStateEnter;
    public static event Action<TBaseState>? OnStateExit;
    // public TContext Context => context;
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
    public abstract class StateBase(FSM2<TContext, TBaseState> fsm)
    {
        public readonly FSM2<TContext, TBaseState> FSM = fsm;
        public virtual void OnExit() { }
        public virtual void OnUpdate(float dt) {}
    }

    // [JsonIgnore] BindDataUpdate selfTick;
    protected readonly TContext Context = context;
    bool isLaunched;
    TBaseState? curState;
    // [JsonIgnore] readonly List<BindDataBase> unbindableInstances = [];
    
    
    protected void Launch<TSubState>() where TSubState : TBaseState
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
        OnStateEnter?.Invoke(curState);
    }
    public bool IsState<TSubState>([NotNullWhen(true)] out TSubState subState)
        where TSubState : TBaseState
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