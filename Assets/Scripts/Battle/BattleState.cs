public class BattleState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("BattleState OnEnter", LogType.State);
        BattleModel.EnterBattle();
        GlobalView.MainView.OnEnterBattleState();
        GlobalView.BattleView.OnEnterGlobalBattle(new OnEnterBattleArg
            {
                PlayerName = MainModel.PlayerName,
                PlayerJob = MainModel.PlayerJob,
                PlayerData = BattleModel.PlayerData,
                MapData = BattleModel.MapData,
            });
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
    }

    protected override void OnExit()
    {   
        MyDebug.Log("SelectNextRoomState OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        
    }
}

public class InRoomBattleState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("InRoomBattleState OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("InRoomBattleState OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        
    }
    
}



