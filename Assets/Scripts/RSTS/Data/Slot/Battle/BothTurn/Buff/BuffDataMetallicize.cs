using System;

namespace RSTS;

[Serializable]
public class BuffDataMetallicize : BuffDataBase
{
    public override string Name => "金属化";
    public override EBuffUseTime UseTime => EBuffUseTime.TurnEnd;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.None;
    protected override EBuffDisposeType DisposeType => EBuffDisposeType.Never;
    public override bool HasStack => true;
    public override void Use(HPAndBuffData hpAndBuffData)
    {
        hpAndBuffData.Block.Value += StackCount;
    }
}