using System.Collections.Generic;
using System;

public class MyFSM
{       
    Dictionary<Type, MyStateBase> stateDic;
    public MyStateBase GetStateByType(Type stateType)
    {
        if (stateType == null)
        {
            return null;
        }
        if (stateDic.ContainsKey(stateType))
        {
            return stateDic[stateType];
        }
        MyStateBase state = Activator.CreateInstance(stateType) as MyStateBase;
        stateDic.Add(stateType, state);
        return state;
    }
    MyStateBase curState;

    public void Launch(Type startStateType)
    {
        stateDic = new();
        curState = GetStateByType(startStateType);
        curState?.Enter();
    }
    public void Update()
    {
        curState?.Update();
    }
    public void ChangeState(Type newStateType)
    {
        if (curState == null)
        {
            Launch(newStateType);
            return;
        }
        MyStateBase newState = GetStateByType(newStateType);
        curState?.Exit();
        curState = newState;
        curState.Enter();
    }
    
}
