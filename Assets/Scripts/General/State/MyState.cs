using System;

public class MyState
{
    public void Enter()
    {
        OnEnter?.Invoke();
    }
    public void Exit()
    {
        OnExit?.Invoke();
    }
    public void Update()
    {
        OnUpdate?.Invoke();
    }
    public event Action OnUpdate;
    public event Action OnEnter;
    public event Action OnExit;
}