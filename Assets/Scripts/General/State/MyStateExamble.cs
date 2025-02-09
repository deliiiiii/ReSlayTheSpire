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
        MyDebug.Log("MyStateExamble OnUpdate", LogType.State);
    }
}