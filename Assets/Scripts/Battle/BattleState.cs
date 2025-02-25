public class BattleState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("BattleState OnEnter", LogType.State);
        BattleModel.EnterGlobalBattle();
    }

    protected override void OnExit()
    {
        MyDebug.Log("BattleState OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
    }
}


public class SelectNextRoomState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("SelectNextRoomState OnEnter", LogType.State);
        MyEvent.Fire(new OnEnterSelectNextRoomStateEvent(BattleModel.CurSelectedEnemyType));
    }

    protected override void OnExit()
    {   
        MyDebug.Log("SelectNextRoomState OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        
    }
}

public class OnEnterSelectNextRoomStateEvent
{
    private OnEnterSelectNextRoomStateEvent(){}
    public OnEnterSelectNextRoomStateEvent(string f_enemyType)
    {
        EnemyType = f_enemyType;
    }
    public string EnemyType;
}



