using System;
using Violee;

public enum EBuffEffectType
{
    EffectOnCloseWindow,
    Consistent,
}

[Serializable]
public class BuffData : DataBase
{
    public required string Des;
    public required Action BuffEffect;
    public required EBuffEffectType EffectType;
}