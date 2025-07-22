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
    public void Update(float dt)
    {
        OnUpdate?.Invoke(dt);
    }
    public event Action<float> OnUpdate;
    public event Action OnEnter;
    public event Action OnExit;
}