public class OnClickStartCommand : AbstractCommand
{
    public override void OnExecute()
    {
        MainModel.SetState(typeof(WaitForStartState),typeof(WaitForStartState_SelectJob));
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

