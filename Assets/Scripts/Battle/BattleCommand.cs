public class OnConfirmExitBattleCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.SetState(typeof(WaitForStartState_Title));
    }
}


public class OnSetNextRoomEnemyCommand : AbstractCommand
{
    public string EnemyType;
    public override void OnExecute()
    {
        BattleModel.SetCurSelectedEnemyType(EnemyType);
    }
}


public class OnEnterNextRoomBattleCommand : AbstractCommand
{
    public override void OnExecute()
    {
        BattleModel.EnterNextRoomBattle();
    }
}