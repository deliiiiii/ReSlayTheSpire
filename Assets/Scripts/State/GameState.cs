public class TitleState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("TitleState OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("TitleState OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        
    }
}

public class SelectState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("GameState OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("GameState OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        
    }
}