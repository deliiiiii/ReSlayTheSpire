using SelectJobModel = MainModel.SelectJobModel;
public class OnClickNextJobCommand : AbstractCommand
{
    public override void OnExecute()
    {
        SelectJobModel.SetSelectedJob(SelectJobModel.GetNextJob());
    }
}

public class OnClickLastJobCommand : AbstractCommand
{
    public override void OnExecute()
    {
        SelectJobModel.SetSelectedJob(SelectJobModel.GetLastJob());
    }
}


public class OnClickConfirmJobCommand : AbstractCommand
{
    public override void OnExecute()
    {
        if(!SelectJobModel.IsIronClad())
        {
            MainView.Instance.ShowErrorPanel("只有 铁甲战士 才能启动！");
            return;
        }
        MainModel.SetState(typeof(EnterBattleState));
    }
}

public class OnClickCancelJobCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.SetState(typeof(WaitForStartState_Title));
    }
}