using System;

public interface IState
{
    void Enter();
    void Exit();
    void Update(float dt);
}

public interface IStateForView : IState
{
    IStateForView OnEnterAfter(Action act);
    IStateForView OnExitBefore(Action act);
}

public interface IStateForData : IState
{
    IStateForData OnEnter(Action act);
    IStateForData OnExit(Action act);
    IStateForData OnUpdate(Action<float> act);
}