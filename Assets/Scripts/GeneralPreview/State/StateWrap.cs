using System;

public interface IMyFSMArg
{
    public void Launch();
    public void UnInit();
}

public class StateWrap<TEnum, TArg>
    where TEnum : Enum
    where TArg : class, IMyFSMArg
{
    public static StateWrap<TEnum, TArg> One => new();
    protected StateWrap(){}
}