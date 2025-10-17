using System;

public class BindDataState : BindDataBase
{
    readonly MyState state;
    Action onEnter;
    Action<float> onUpdate;
    Action onExit;
    public BindDataState(MyState state)
    {
        this.state = state;
    }

    protected override void BindInternal()
    {
        state.OnEnter += onEnter;
        state.OnUpdate += onUpdate;
        state.OnExit += onExit;
    }

    public override void UnBind()
    {
        // if(onExit != null)
            state.OnEnter -= onEnter;
        // if(onUpdate != null)
            state.OnUpdate -= onUpdate;
        // if(onExit != null)
            state.OnExit -= onExit;
    }
    
    
    public BindDataState OnEnter(Action act)
    {
        onEnter = act;
        return this;
    }
    
    public BindDataState OnExit(Action act)
    {
        onExit = act;
        return this;
    }
    
    public BindDataState OnUpdate(Action<float> act)
    {
        onUpdate = act;
        return this;
    }
}
