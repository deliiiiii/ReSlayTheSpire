using System;

public class BindState
{
    readonly MyState state;
    public BindState(MyState state)
    {
        this.state = state;
    }
    
    public BindState OnEnter(Action act)
    {
        state.OnEnter += act;
        return this;
    }
    
    public BindState OnExit(Action act)
    {
        state.OnExit += act;
        return this;
    }
    
    public BindState OnUpdate(Action<float> act)
    {
        state.OnUpdate += act;
        return this;
    }
}