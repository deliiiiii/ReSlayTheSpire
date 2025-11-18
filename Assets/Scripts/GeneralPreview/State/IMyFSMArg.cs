using System;

public interface IMyFSMArg<out TEnum>
{
    public void Bind(Func<TEnum, MyState> getState);
    public void Launch();
    public void UnInit();
}