using UnityEngine;
using System;
using UnityEngine.UI;
public class MainLogic : MonoBehaviour
{
    MyFSM mainFSM;
    IMain mainUI;

    void Awake()
    {
        mainFSM = new MyFSM(typeof(Main_WaitForStartState));
        mainUI = MainUI.Instance;
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

    void Update()
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



