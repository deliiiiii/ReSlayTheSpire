public class OnConfirmExitBattleCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.ChangeState(typeof(WaitForStartState_Title));
    }
}

public class OnEnterNextRoomBattleCommand : AbstractCommand
{
    public override void OnExecute()
    {
        BattleModel.EnterNextRoomBattle();
    }
}