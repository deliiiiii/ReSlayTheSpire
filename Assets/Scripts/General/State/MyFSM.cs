using System.Collections.Generic;
using System;

public class MyFSM
{
    public MyFSM(Type startStateType)
    {
        Launch(startStateType);
    }

    Dictionary<Type,MyStateBase> stateDic;
    MyStateBase curState;
    MyStateBase CreateStateByType(Type stateType)
    {
        MyStateBase newState = Activator.CreateInstance(stateType) as MyStateBase;
        newState.fsm = this;
        stateDic.Add(stateType,newState);
        return newState;
    }
    void OnStateChange(MyStateBase oldState,MyStateBase newState)
    {
        oldState?.OnExit();
        newState?.OnEnter();
    }
    void Launch(Type startStateStpe)
    {
        stateDic = new();
        curState = CreateStateByType(startStateStpe);
        curState.OnEnter();
    }
    public void Update()
    {
        curState?.OnUpdate();
    }
    public void ChangeState(Type newStateType)
    {
        MyStateBase newState;
        if (!stateDic.ContainsKey(newStateType))
            newState = CreateStateByType(newStateType);
        else
            newState = stateDic[newStateType];
        curState = newState;
        OnStateChange(curState,newState);
    }
}
