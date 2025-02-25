public class OnConfirmExitBattleCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.SetState(typeof(WaitForStartState_Title));
    }
}


public class OnSetNextRoomEnemy : AbstractCommand
{
    public string EnemyType;
    public override void OnExecute()
    {
        BattleModel.SetCurSelectedEnemyType(EnemyType);
    }
}


