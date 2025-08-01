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

    MyState curStateClass;
    [NonSerialized]
    public Enum CurState;
    [ShowInInspector]string curStateName => CurState?.ToString() ?? "Null";
    void Launch(T startState)
    {
        curStateClass = GetState(startState);
        CurState = startState;
        curStateClass.Enter();
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
    
    public void Update(float dt)
    {
        curStateClass?.Update(dt);
    }
    public void ChangeState(T e)
    {
        if (curStateClass == null)
        {
            Launch(e);
            return;
        }
        var newStateClass = GetState(e);
        if (newStateClass == curStateClass)
            return;
        curStateClass.Exit();
        curStateClass = newStateClass;
        CurState = e;
        curStateClass.Enter();
    }
    
    public bool IsState(Enum e) => Equals(CurState, e);
    public bool IsState(Enum e1, Enum e2) => Equals(CurState, e1) || Equals(CurState, e2);
    public bool IsState(Enum e1, Enum e2, Enum e3) => Equals(CurState, e1) || Equals(CurState, e2) || Equals(CurState, e3);
}
