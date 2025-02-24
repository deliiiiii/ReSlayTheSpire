public class OnConfirmExitBattleCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.SetState(typeof(WaitForStartState), typeof(WaitForStartState_Title));
    }
}





