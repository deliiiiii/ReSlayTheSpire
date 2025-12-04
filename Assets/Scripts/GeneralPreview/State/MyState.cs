using System;

public class MyStateForView
{
    internal Action? OnEnterAfterEvt;
    internal Action? OnExitBeforeEvt;
    
    public MyStateForView OnEnterAfter(Action act)
    {
        OnEnterAfterEvt += act;
        return this;
    }

    public MyStateForView OnExitBefore(Action act)
    {
        OnExitBeforeEvt += act;
        return this;
    }
}

public class MyState : MyStateForView
{
    internal void Enter()
    {
        OnEnterEvt?.Invoke();
        OnEnterAfterEvt?.Invoke();
    }
    internal void Exit()
    {
        OnExitBeforeEvt?.Invoke();
        OnExitEvt?.Invoke();
    }
    internal void Update(float dt)
    {
        OnUpdateEvt?.Invoke(dt);
    }
    internal event Action<float>? OnUpdateEvt;
    internal event Action? OnEnterEvt;
    internal event Action? OnExitEvt;

    [Obsolete("In class XXXData, use OnEnter instead.")]
    public new MyStateForView OnEnterAfter(Action act) => this;
    [Obsolete("In class XXXData, use OnExit instead.")]
    public new MyStateForView OnExitBefore(Action act) => this;
    public MyState OnEnter(Action act)
    {
        OnEnterEvt += act;
        return this;
    }
    public MyState OnExit(Action act)
    {
        OnExitEvt += act;
        return this;
    }

    public MyState OnUpdate(Action<float> act)
    {
        OnUpdateEvt += act;
        return this;
    }
}