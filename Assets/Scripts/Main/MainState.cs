using System;
using UnityEngine;
public class ConstUICard
{
    public SerializableDictionary<CardColor, Color> cardColorToImageColor;
    public string ResPath { get; set; }
}

public class WaitForStartState_Title : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("WaitForStartState_Title OnEnter", LogType.State);
        MyEvent.Fire(new OnEnterTitleStateEvent());
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
        MyEvent.Fire(new OnEnterSelectJobStateEvent()
        {
            JobType = MainModel.SelectJobModel.SelectedJob,
        });
    }

    protected override void OnExit()
    {
        MyDebug.Log("WaitForStartState_SelectJob OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        
    }
}

public class OnEnterTitleStateEvent
{

}

public class OnEnterSelectJobStateEvent
{
    public JobType JobType;
}