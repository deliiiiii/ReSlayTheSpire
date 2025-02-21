using System;
using UnityEngine;

public class MyStateExamble : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("MyStateExamble OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("MyStateExamble OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // GameManager.gameFSM.ChangeState(typeof(MyStateExamble2));
        }
    }
}

public class MyStateExamble2 : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("MyStateExamble2 OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("MyStateExamble2 OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // GameManager.gameFSM.ChangeState(typeof(MyStateExamble));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // GameManager.gameFSM.ChangeState(typeof(MyStateExamble3));
        }
    }
}

public class MyStateExamble3 : MyStateBase
{
    public override Type GetDefaultSubStateType()
    {
        return typeof(MyStateExamble3_1);
    }
    protected override void OnEnter()
    {
        MyDebug.Log("MyStateExamble3 OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("MyStateExamble3 OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // GameManager.gameFSM.ChangeState(typeof(MyStateExamble2));
        }
    }
}
public class MyStateExamble3_1 : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("MyStateExamble3_1 OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("MyStateExamble3_1 OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // GameManager.gameFSM.ChangeState(typeof(MyStateExamble3), typeof(MyStateExamble3_2));
        }
    }
}

public class MyStateExamble3_2 : MyStateBase
{
    protected override void OnEnter()
    {
        MyDebug.Log("MyStateExamble3_2 OnEnter", LogType.State);
    }

    protected override void OnExit()
    {
        MyDebug.Log("MyStateExamble3_2 OnExit", LogType.State);
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // GameManager.gameFSM.ChangeState(typeof(MyStateExamble3), typeof(MyStateExamble3_1));
        }
    }
}