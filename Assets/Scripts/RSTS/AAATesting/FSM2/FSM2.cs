using System;
using System.Diagnostics.CodeAnalysis;

namespace RSTS;

public abstract class FSM2<TThis>
    where TThis : FSM2<TThis>
{
    public static event Action<IState>? OnStateEnter;
    public static event Action<IState>? OnStateExit;
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
    
    bool isLaunched;
    IState? curState;
    // [JsonIgnore] readonly List<BindDataBase> unbindableInstances = [];

    public void Launch<TSubState>() where TSubState : class, IState
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
    
    public void EnterState<TSubState>() where TSubState : class, IState
    {
        if (!isLaunched)
        {
            MyDebug.LogError($"{GetType().Name} Enter State But NOT Launched");
            return;
        }
        if(curState != null)
            OnStateExit?.Invoke(curState);
        curState?.OnExit();
        curState = Activator.CreateInstance<TSubState>();
        curState.BelongFSM = (TThis)this;
        curState?.OnEnter();
        OnStateEnter?.Invoke(curState);
    }
    public bool IsState<TSubState>([NotNullWhen(true)] out TSubState subState)
        where TSubState : class, IState
    {
        subState = null!;
        if (curState is TSubState state)
        {
            subState = state;
            return true;
        }
        return false;
    }
    
    public interface IState
    {
        public TThis BelongFSM { get; internal set; }
        public void OnEnter(){}
        public void OnExit(){}
        public void OnUpdate(float dt){}
    }
}
public abstract class FSM2<TOuter, TThis> : FSM2<TThis>
    where TThis : FSM2<TOuter, TThis>
{
    public required TOuter Outer;
}