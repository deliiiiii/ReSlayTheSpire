using System;

public interface IMyFSMArg
{
    public void Launch();
    public void UnInit();
}

[Serializable]
public abstract class StateWrap
{
    public StateWrap? Parent;
}

[Serializable]
public class StateWrap<TEnum, TArg> : StateWrap
    where TEnum : struct, Enum
    where TArg : class, IMyFSMArg
{
    public static StateWrap<TEnum, TArg> One => new();
    // public static StateWrap<TEnum, TArg> One => new(default, null!);
    protected StateWrap()
    {
        Arg = null!;
        Fsm = null!;
        SelfTick = null!;
    }
    internal TArg Arg;
    internal MyFSM<TEnum> Fsm;
    internal BindDataUpdate SelfTick;
}