using System;
using Violee;

[Serializable]
public class BuffData : DataBase
{
    public required string Des;
    public required Action BuffEffect;
}