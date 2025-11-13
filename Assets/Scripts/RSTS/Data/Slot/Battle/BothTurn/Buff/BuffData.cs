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
    public virtual void Use(HPAndBuffData hpAndBuffData){}
    public virtual bool Dispose() => false;
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
