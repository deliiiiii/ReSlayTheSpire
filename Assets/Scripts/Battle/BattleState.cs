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
        throw new System.NotImplementedException();
    }
}
