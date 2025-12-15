using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace RSTS;

public abstract class FSM2<TThis>
    where TThis : FSM2<TThis>
{
    public static event Action<IState>? OnStateEnter;
    public static event Action<IState>? OnStateExit;
    bool isLaunched;
    IState? curState;
    [JsonIgnore] BindDataUpdate? selfTickBind;

    public void Launch<TSubState>() where TSubState : class, IState
    {
        if (isLaunched)
        {
            MyDebug.LogError($"FSM {GetType().Name} Has Already Launched");
            return;
        }
        isLaunched = true;
        EnterState<TSubState>();
        selfTickBind = Binder.FromTick(Tick);
        selfTickBind.Bind();
    }
    public void Release()
    {
        if (!isLaunched)
        {
            MyDebug.LogError($"FSM {GetType().Name} Release But NOT Launched"); 
            return;
        }
        isLaunched = false;
        curState?.OnExit();
        curState = null;
        selfTickBind?.UnBind();
        selfTickBind = null;
    }
    public void EnterState<TSubState>() where TSubState : class, IState
    {
        if (!isLaunched)
        {
            MyDebug.LogError($"FSM {GetType().Name} Enter State But NOT Launched");
            return;
        }
        if (curState != null)
        {
            OnStateExit?.Invoke(curState);
            curState.OnExit();
        }
        curState = Activator.CreateInstance<TSubState>()!;
        curState.BelongFSM = (TThis)this;
        curState.OnEnter();
        OnStateEnter?.Invoke(curState);
    }
    public bool IsState<TSubState>([NotNullWhen(true)] out TSubState subState) where TSubState : class, IState
    {
        subState = null!;
        if (curState is TSubState state)
        {
            subState = state;
            return true;
        }
        return false;
    }
    protected virtual void Tick(float dt) => curState?.OnUpdate(dt);

    public interface IState
    {
        public TThis BelongFSM { get; set; }
        public void OnEnter(){}
        public void OnExit(){}
        public void OnUpdate(float dt){}
    }
}