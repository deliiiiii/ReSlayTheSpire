using System;

namespace RSTS;

[Serializable]
public class BuffFlameBarrier : BuffDataBase
{
    public override string Name => "火焰屏障";
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.TurnEnd;
    protected override EBuffDisposeType DisposeType => EBuffDisposeType.AllStack;
}