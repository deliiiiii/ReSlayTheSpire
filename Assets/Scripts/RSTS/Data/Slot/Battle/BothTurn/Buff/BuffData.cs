using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RSTS;
[Serializable]
public enum EBuffUseTime
{
    None,
    TurnStart,
    TurnEnd,
}
[Serializable]
public enum EBuffDisposeTime
{
    None,
    TurnStart,
    TurnEnd,
}

[Serializable]
public enum EBuffDisposeType
{
    Never,
    OneStack,
    AllStack,
    Other,
}

[Serializable]
public class BuffStackInfo
{
    public Observable<int> Count = new(1);
}

[Serializable]
public abstract class BuffDataBase
{
    [SerializeReference]
    public BuffStackInfo? StackInfo;
    public abstract string Name { get; }
    public abstract EBuffUseTime UseTime { get; }
    public abstract EBuffDisposeTime DisposeTime { get; }
    protected abstract EBuffDisposeType DisposeType { get; }
    public virtual void Use(HPAndBuffData hpAndBuffData){}

    public bool Dispose()
    {
        return DisposeType switch
        {
            EBuffDisposeType.OneStack => DisposeOneStack(),
            EBuffDisposeType.AllStack => DisposeAllStacks(),
            EBuffDisposeType.Never => CustomDispose(),
            _ => false,
        };
    }
    protected virtual bool CustomDispose() => false;

    bool DisposeOneStack()
    {
        StackInfo!.Count.Value--;
        return StackInfo.Count.Value <= 0;
    }

    bool DisposeAllStacks()
    {
        StackInfo!.Count.Value = 0;
        return true;
    }
}


public interface IBuffFromAtkBaseAdd
{
    int GetAtkBaseAdd();
}

public interface IBuffAtkFinalMul
{
    float GetAtkFinalMulti();
}

public interface IBuffFromAtkFinalMul : IBuffAtkFinalMul;
public interface IBuffToAtkFinalMul : IBuffAtkFinalMul;
