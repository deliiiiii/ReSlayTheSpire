using System;

namespace RSTS;

[Serializable]
public class BuffDataMetallicize : BuffDataBase
{
    public override string Name => "金属化";
    public override EBuffUseTime UseTime => EBuffUseTime.TurnEnd;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.None;

    public override void Use(HPAndBuffData hpAndBuffData)
    {
        hpAndBuffData.Block.Value += StackInfo?.Count.Value ?? 0;
    }
}