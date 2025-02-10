using System;

public abstract class MyStateBase
{
    protected MyStateBase subState;
    public virtual Type GetDefaultSubStateType() { return null; }
    public virtual void Enter(MyStateBase newSubState, bool isEnterSame)
    {
        if (!isEnterSame)
        {
            OnEnter();
        }
        if (newSubState == null)
        {
            return;
        }
        ChangeSubState(newSubState);
    }
    void ChangeSubState(MyStateBase newSubState)
    {
        subState?.Exit();
        subState = newSubState;
        subState?.Enter(null, false);
    }
    public virtual void Exit()
    {
        subState?.Exit();
        subState = null;
        OnExit();
    }
    public virtual void Update()
    {
        OnUpdate();
        subState?.Update();
    }
    protected abstract void OnUpdate();
    protected abstract void OnEnter();
    protected abstract void OnExit();
}