using QFramework;
using UnityEngine;
public class OnClickStartCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetSystem<GlobalSystem>().ChangeState(typeof(WaitForStartState),typeof(WaitForStartState_SelectJob));
    }
}

public class OnClickConfirmJobCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        //TODO: 确认职业
        this.GetSystem<GlobalSystem>().ChangeState(typeof(WaitForStartState),typeof(WaitForStartState_Title));
    }
}

public class OnClickCancelJobCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetSystem<GlobalSystem>().ChangeState(typeof(WaitForStartState),typeof(WaitForStartState_Title));
    }
}



public class OnClickQuitCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        UnityEngine.Application.Quit();
        #endif
    }
}

