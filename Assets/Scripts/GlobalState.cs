using System;
using UnityEngine;
public class ConstUICard
{
    public SerializableDictionary<Deli.CardColor, Color> cardColorToImageColor;
    public Font cardNameFont;
    public Font cardCostFont;
    public Font cardDescriptionFont;
    public string ResPath { get; set; }
}
public class MainLogic
{
    MyFSM mainFSM;
    // MainUI mainUI;
    public MainLogic(MainUI mainUI)
    {
        // mainFSM = new MyFSM(typeof(Main_WaitForStartState));
        mainUI.OnClickStart += () => 
        {
            // mainFSM.ChangeState(typeof(Main_SelectJobState));
            mainUI.OnLogicStart?.Invoke();
        };
        mainUI.OnClickQuit += () => 
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            UnityEngine.Application.Quit();
            #endif
        };
    }

    public void Update()
    {
        mainFSM.Update();
    }

}




[Serializable]
public class StateData
{
    public Type StateType;
}

[Serializable]
public class WaitForStartData : StateData
{
    public Type subStateType;
    public JobType selectedJob;
}

public class WaitForStartState : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("WaitForStartState OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("WaitForStartState OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
         
    }
}

public class WaitForStartState_Title : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("WaitForStartState_Title OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("WaitForStartState_Title OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        
    }
}

public class WaitForStartState_SelectJob : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("WaitForStartState_SelectJob OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("WaitForStartState_SelectJob OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        
    }
}