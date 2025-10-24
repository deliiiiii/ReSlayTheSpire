using System;

public interface IMyFSMArg
{
    public void Launch();
    public void UnInit();
}

public class StateWrapper<TEnum, TArg>
    where TEnum : Enum
    where TArg : class, IMyFSMArg
{
    public static StateWrapper<TEnum, TArg> One => new();
    protected StateWrapper(){}
}