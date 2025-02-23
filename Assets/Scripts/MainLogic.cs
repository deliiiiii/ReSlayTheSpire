public class MainLogic
{
    MyFSM mainFSM;
    // MainUI mainUI;
    public MainLogic(MainUI mainUI)
    {
        mainFSM = new MyFSM(typeof(Main_WaitForStartState));
        // this.mainUI = mainUI;
        mainUI.OnClickStart += () => 
        {
            mainFSM.ChangeState(typeof(Main_SelectJobState));
            mainUI.OnLogicStart?.Invoke();
        };
        mainUI.OnClickQuit += () => 
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        };
    }

    public void Update()
    {
        mainFSM.Update();
    }

}



public class Main_WaitForStartState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("Main_WaitForStartState OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("Main_WaitForStartState OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
         
    }
}

public class Main_SelectJobState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("Main_SelectJobState OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("Main_SelectJobState OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        
    }
}



