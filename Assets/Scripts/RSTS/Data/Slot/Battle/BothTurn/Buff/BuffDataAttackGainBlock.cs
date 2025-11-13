using System;

namespace RSTS;

[Serializable]
public class BuffDataAttackGainBlock : BuffDataBase
{
    public override string Name => "愤怒";
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.TurnEnd;
    protected override EBuffDisposeType DisposeType => EBuffDisposeType.AllStack;
}