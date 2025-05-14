using System;
using UnityEngine;
public class WaitForStartState_Title : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("WaitForStartState_Title OnEnter", LogType.State);
        GlobalView.MainView.OnEnterTitleState();
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
