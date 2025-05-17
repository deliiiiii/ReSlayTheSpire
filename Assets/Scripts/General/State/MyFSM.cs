using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class MyFSM<T> where T : Enum
{       
    Dictionary<string, MyState> stateDic;

    public MyFSM()
    {
        stateDic = new Dictionary<string, MyState>();
        foreach (var e in Enum.GetValues(typeof(T)))
        {
            stateDic.Add(e.ToString(), new MyState());
        }
    }
    public MyState GetState(string stateType)
    {
        if (stateType == null)
        {
            return null;
        }
        if (stateDic.TryGetValue(stateType, out var value))
        {
            return value;
        }
        MyState state = new();
        stateDic.Add(stateType, state);
        return state;
    }
    MyState curState;
    [ShowInInspector]string curStateName => curState.GetType().ToString();
    void Launch(string startStateType)
    {
        curState = GetState(startStateType);
        curState?.Enter();
    }
    public void Update()
    {
        curState?.Update();
    }
    public void ChangeState(string newStateType)
    {
        if (curState == null)
        {
            Launch(newStateType);
            return;
        }
        MyState newState = GetState(newStateType);
        curState?.Exit();
        curState = newState;
        curState.Enter();
    }
    
}
