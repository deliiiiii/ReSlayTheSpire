using System;

public abstract class MyStateBase
{
    public virtual void Enter()
    {
        OnEnter();
    }
    public virtual void Exit()
    {
        OnExit();
    }
    public virtual void Update()
    {
        OnUpdate();
    }
    protected abstract void OnUpdate();
    protected abstract void OnEnter();
    protected abstract void OnExit();
}