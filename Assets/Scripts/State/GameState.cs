public class WaitForStartState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("WaitForStartState OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("WaitForStartState OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        
    }
}

public class SelectJobState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("SelectJobState OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("SelectJobState OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        
    }
}