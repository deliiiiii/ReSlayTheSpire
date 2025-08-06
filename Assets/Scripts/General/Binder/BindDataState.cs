using System;

public class BindDataState
{
    internal MyState state;
    public BindDataState(MyState state)
    {
        this.state = state;
    }

   
    
    //TODO UnBind()
}

public static class BindDataStateExt
{
    public static BindDataState OnEnter(this BindDataState self, Action act)
    {
        self.state.OnEnter += act;
        return self;
    }
    
    public static BindDataState OnExit(this BindDataState self, Action act)
    {
        self.state.OnExit += act;
        return self;
    }
    
    public static BindDataState OnUpdate(this BindDataState self, Action<float> act)
    {
        self.state.OnUpdate += act;
        return self;
    }
    public static BindDataState RemoveUpdate(this BindDataState self, Action<float> act)
    {
        self.state.OnUpdate -= act;
        return self;
    }
}