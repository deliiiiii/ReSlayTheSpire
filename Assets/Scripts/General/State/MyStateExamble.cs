using System;
using UnityEngine;

public class MyStateExamble : MyStateBase
{
    public override void OnEnter()
    {
        MyDebug.Log("MyStateExamble OnEnter", LogType.State);
    }

    public override void OnExit()
    {
        MyDebug.Log("MyStateExamble OnExit", LogType.State);
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            belongFSM.ChangeState(typeof(MyStateExamble2));
        }
    }
}

public class MyStateExamble2 : MyStateBase
{
    public override void OnEnter()
    {
        MyDebug.Log("MyStateExamble2 OnEnter", LogType.State);
    }

    public override void OnExit()
    {
        MyDebug.Log("MyStateExamble2 OnExit", LogType.State);
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            belongFSM.ChangeState(typeof(MyStateExamble));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            belongFSM.ChangeState(typeof(MyStateExamble3));
        }
    }
}

public class MyStateExamble3 : MyStateBase
{
    public override Type GetDefaultSubStateType()
    {
        return typeof(MyStateExamble3_1);
    }
    public override void OnEnter()
    {
        MyDebug.Log("MyStateExamble3 OnEnter", LogType.State);
    }

    public override void OnExit()
    {
        MyDebug.Log("MyStateExamble3 OnExit", LogType.State);
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            belongFSM.ChangeState(typeof(MyStateExamble2));
        }
    }
}
public class MyStateExamble3_1 : MyStateBase
{
    public override void OnEnter()
    {
        MyDebug.Log("MyStateExamble3_1 OnEnter", LogType.State);
    }

    public override void OnExit()
    {
        MyDebug.Log("MyStateExamble3_1 OnExit", LogType.State);
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            belongFSM.ChangeState(typeof(MyStateExamble3), typeof(MyStateExamble3_2));
        }
    }
}

public class MyStateExamble3_2 : MyStateBase
{
    public override void OnEnter()
    {
        MyDebug.Log("MyStateExamble3_2 OnEnter", LogType.State);
    }

    public override void OnExit()
    {
        MyDebug.Log("MyStateExamble3_2 OnExit", LogType.State);
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            belongFSM.ChangeState(typeof(MyStateExamble3), typeof(MyStateExamble3_1));
        }
    }
}