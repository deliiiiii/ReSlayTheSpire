using System;
using Violee;


[Serializable]
public class BuffData : DataBase
{
    public required Func<string> GetDes;
}

[Serializable]
public class WindowBuffData : BuffData
{
    public required Action BuffEffect;
}

[Serializable]
public class ConsistentBuffData : BuffData
{
    public EConBuffType ConBuffType;
}

public enum EConBuffType
{
    PlayRecord,
}