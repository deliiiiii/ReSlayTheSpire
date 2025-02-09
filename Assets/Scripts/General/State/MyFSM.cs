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
        if (stateDic.ContainsKey(stateType))
        {
            return stateDic[stateType];
        }
        MyStateBase state = Activator.CreateInstance(stateType) as MyStateBase;
        state.belongFSM = this;
        stateDic.Add(stateType, state);
        return state;
    }
    MyStateBase curState;

    void Launch(Type startStateType, Type subStateType = null)
    {
        stateDic = new();
        curState = GetStateByType(startStateType);
        curState.Enter(subStateType);
    }
    public void Update()
    {
        curState?.Update();
    }
    public void ChangeState(Type newStateType, Type subStateType = null)
    {
        if(curState.GetType() != newStateType)
        {
            curState.Exit();
            curState = GetStateByType(newStateType);
            curState.Enter(subStateType);
            return;
        }
        curState.ChangeSubState(subStateType);
    }
    
}
