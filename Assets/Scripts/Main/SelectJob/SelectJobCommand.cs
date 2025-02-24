public class OnClickNextJobCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.SelectJobModel.SetSelectedJob(MainModel.SelectJobModel.GetNextJob());
    }
}

public class OnClickLastJobCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.SelectJobModel.SetSelectedJob(MainModel.SelectJobModel.GetLastJob());
    }
}


public class OnClickConfirmJobCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.SetState(typeof(WaitForStartState),typeof(WaitForStartState_Title));
    }
}

public class OnClickCancelJobCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.SetState(typeof(WaitForStartState),typeof(WaitForStartState_Title));
    }
}