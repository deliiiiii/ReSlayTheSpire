using System.Collections.Generic;
using System;

public class MyFSM
{   
    public MyFSM(Type initialStateType)
    {
        if (initialStateType == null)
        {
            //没有子状态的状态机
            return;
        }
        Launch(initialStateType);
    }
    
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

    void Launch(Type startStateType, Type subStateType = null)
    {
        stateDic = new();
        curState = GetStateByType(startStateType);
        curState.Enter(GetStateByType(subStateType), isEnterSame: false);
    }
    public void Update()
    {
        curState?.Update();
    }
    public void ChangeState(Type newStateType, Type subStateType = null)
    {
        MyStateBase newState = GetStateByType(newStateType);
        if(subStateType == null)
        {
            subStateType = newState.GetDefaultSubStateType();
        }
        MyStateBase newSubState = GetStateByType(subStateType);
        bool isEnterSame = curState == newState;
        curState.Exit();
        curState = newState;
        curState.Enter(newSubState, isEnterSame);
    }
    
}
