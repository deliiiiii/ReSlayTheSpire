using System;

public abstract class MyStateBase
{
    protected MyStateBase subState;
    public virtual Type GetDefaultSubStateType() { return null; }
    public MyFSM belongFSM;
    public virtual void Enter(Type newSubStateType = null)
    {
        OnEnter();
        if (newSubStateType == null)
        {
            newSubStateType = GetDefaultSubStateType();
            if (newSubStateType == null)
            {
                return;
            }
        }
        ChangeSubState(newSubStateType);
    }
    public void ChangeSubState(Type newSubStateType)
    {
        subState?.Exit();
        subState = belongFSM.GetStateByType(newSubStateType);
        subState.belongFSM = belongFSM;
        subState.Enter();
    }
    public virtual void Exit()
    {
        if (subState != null)
        {
            subState.Exit();
            subState = null;
        }
        OnExit();
    }
    public virtual void Update()
    {
        OnUpdate();
        subState?.Update();
    }
    public abstract void OnUpdate();
    public abstract void OnEnter();
    public abstract void OnExit();
    
    
    
    
}