using System;

public class BindDataState
{
    MyState state;
    public BindDataState(MyState state)
    {
        this.state = state;
    }

    public BindDataState OnEnter(Action act)
    {
        state.OnEnter -= act;
        state.OnEnter += act;
        return this;
    }
    
    public BindDataState OnExit(Action act)
    {
        state.OnExit -= act;
        state.OnExit += act;
        return this;
    }
    
    public BindDataState OnUpdate(Action act)
    {
        state.OnUpdate -= act;
        state.OnUpdate += act;
        return this;
    }
    
    //TODO UnBind()
}