

using System;
using UnityEngine;

namespace RSTS;
[Serializable]
public enum EBuffUseTime
{
    
}
[Serializable]
public enum EBuffDisposeTime
{
    
}

public abstract class BuffStackInfo;
public class BuffCanStack : BuffStackInfo
{
    public int Count;
    public void ChangeCount(int delta)
    {
        Count += delta;
    }
}
public class BuffStackNone : BuffStackInfo;

[Serializable]
public abstract class BuffDataBase
{
    public string Name;
    [SerializeReference]
    public BuffStackInfo StackInfo;
    public abstract EBuffUseTime UseTime { get; }
    public abstract EBuffDisposeTime DisposeTime { get; }
}