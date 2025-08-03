using System;
using JetBrains.Annotations;

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
        OnLateUpdate?.Invoke(dt);
    }
    [CanBeNull] public event Action<float> OnUpdate;
    [CanBeNull] public event Action<float> OnLateUpdate;
    [CanBeNull] public event Action OnEnter;
    [CanBeNull] public event Action OnExit;
}