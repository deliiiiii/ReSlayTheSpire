using System;

public interface IMyFSMArg{}

public class StateWrapper<TEnum, TArg>
    where TEnum : Enum
    where TArg : class, IMyFSMArg
{
    public static StateWrapper<TEnum, TArg> One => new();
}