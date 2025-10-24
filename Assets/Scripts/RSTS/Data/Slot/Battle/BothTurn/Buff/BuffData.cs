

using System;
using UnityEngine;

namespace RSTS;
[Serializable]
public enum EBuffUseTime
{
    None,
}
[Serializable]
public enum EBuffDisposeTime
{
    TurnEnd,
}

[Serializable]
public class BuffStackInfo
{
    public int Count;
    public void ChangeCount(int delta)
    {
        Count += delta;
    }
}

[Serializable]
public abstract class BuffDataBase
{
    [SerializeReference]
    public required BuffStackInfo? StackInfo;
    public abstract EBuffUseTime UseTime { get; }
    public abstract EBuffDisposeTime DisposeTime { get; }
}

[Serializable]
public class BuffDataVulnerable : BuffDataBase
{
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.TurnEnd;
    
    public BuffDataVulnerable(int count)
    {
        StackInfo = new BuffStackInfo { Count = count };
    }
}