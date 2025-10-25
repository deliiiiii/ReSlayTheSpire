using System;
using UnityEngine;

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
public class BuffStackInfo(int count)
{
    public Observable<int> Count = new(count);
    public void ChangeCount(int delta)
    {
        Count.Value += delta;
    }
}

[Serializable]
public abstract class BuffDataBase
{
    [SerializeReference]
    public BuffStackInfo? StackInfo;
    public abstract string Name { get; }
    public abstract EBuffUseTime UseTime { get; }
    public abstract EBuffDisposeTime DisposeTime { get; }
    public virtual bool Dispose() => false;
}


public interface IBuffAtkBaseAdd
{
    int GetAtkBaseAdd();
}

public interface IBuffAtkFinalMul
{
    float GetAtkFinalMulti();
}