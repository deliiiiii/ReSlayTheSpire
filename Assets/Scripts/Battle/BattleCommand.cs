public class OnConfirmExitBattleCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.SetState(typeof(WaitForStartState_Title));
    }
}





