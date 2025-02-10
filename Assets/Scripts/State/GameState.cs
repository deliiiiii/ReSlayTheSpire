public class TitleState : MyStateBase
{
    public override void OnEnter()
    {
        MyDebug.Log("TitleState OnEnter", LogType.State);
    }

    public override void OnExit()
    {
        MyDebug.Log("TitleState OnExit", LogType.State);
    }

    public override void OnUpdate()
    {
        
    }
}

public class SelectState : MyStateBase
{
    public override void OnEnter()
    {
        MyDebug.Log("GameState OnEnter", LogType.State);
    }

    public override void OnExit()
    {
        MyDebug.Log("GameState OnExit", LogType.State);
    }

    public override void OnUpdate()
    {
        
    }
}