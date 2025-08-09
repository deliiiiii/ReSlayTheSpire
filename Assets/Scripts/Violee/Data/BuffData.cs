using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Violee;


[Serializable]
public class BuffData : DataBase
{
    public required string Des;
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
    public bool HasCount;
    [ShowIf(nameof(HasCount))] public ObservableInt Count = new(0);
}

public enum EConBuffType
{
    PlayRecord,
    Lamp,
    SmallLamp,
    Cooler,
}
