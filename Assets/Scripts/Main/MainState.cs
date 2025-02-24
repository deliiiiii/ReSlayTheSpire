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