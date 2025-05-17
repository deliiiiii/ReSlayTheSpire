using System;

public class BindDataEvent
{
    Action evtAction;
    public BindDataEvent(Action evtAction)
    {
        this.evtAction = evtAction;
    }

    public BindDataEvent To(Action act)
    {
        evtAction += act;
        return this;
    }
    
    //TODO UnBind
    
}