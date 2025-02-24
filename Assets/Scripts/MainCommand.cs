public class OnClickStartCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.SetState(typeof(WaitForStartState),typeof(WaitForStartState_SelectJob));
    }
}

public class OnClickConfirmJobCommand : AbstractCommand
{
    public override void OnExecute()
    {
        //TODO: 确认职业
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



public class OnClickQuitCommand : AbstractCommand
{
    public override void OnExecute()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        UnityEngine.Application.Quit();
        #endif
    }
}

