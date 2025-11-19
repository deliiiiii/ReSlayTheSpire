using System;

public interface IMyFSMArg<in TSub>
{
    public void Init();
    public void Bind(TSub fsm);
    public void Launch();
    public void UnInit();
}