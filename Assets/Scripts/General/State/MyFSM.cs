using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class MyFSM<T> where T : Enum
{       
    Dictionary<T, MyState> stateDic;

    public MyFSM()
    {
        stateDic = new Dictionary<T, MyState>();
        foreach (var e in Enum.GetValues(typeof(T)))
        {
            stateDic.Add((T)e, new MyState());
        }
    }
    public MyState GetState(T e)
    {
        if (e == null)
        {
            return null;
        }
        if (stateDic.TryGetValue(e, out var value))
        {
            return value;
        }
        MyState state = new();
        stateDic.Add(e, state);
        return state;
    }
    MyState curState;
    [ShowInInspector]
    Enum curStateEnum;
    void Launch(T startState)
    {
        curState = GetState(startState);
        curStateEnum = startState;
        curState?.Enter();
    }
    public void Update(float dt)
    {
        curState?.Update(dt);
    }
    public void ChangeState(T e)
    {
        if (curState == null)
        {
            Launch(e);
            return;
        }
        MyState newState = GetState(e);
        curState?.Exit();
        curState = newState;
        curStateEnum = e;
        curState.Enter();
    }
    
    public bool IsState(Enum e) => Equals(curStateEnum, e);
    public bool IsState(Enum e1, Enum e2) => Equals(curStateEnum, e1) || Equals(curStateEnum, e2);
    public bool IsState(Enum e1, Enum e2, Enum e3) => Equals(curStateEnum, e1) || Equals(curStateEnum, e2) || Equals(curStateEnum, e3);
}
