public class EnterBattleState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("EnterBattleState OnEnter", LogType.State);
        BattleModel.EnterBattle();
    }

    protected override void OnExit()
    {
        MyDebug.Log("EnterBattleState OnExit", LogType.State);
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