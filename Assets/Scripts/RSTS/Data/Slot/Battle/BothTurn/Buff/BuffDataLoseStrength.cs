using System;

namespace RSTS;

[Serializable]
public class BuffDataLoseStrength : BuffDataBase
{
    public override string Name => "活动肌肉";
    public override EBuffUseTime UseTime => EBuffUseTime.TurnEnd;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.TurnEnd;
    protected override EBuffDisposeType DisposeType => EBuffDisposeType.AllStack;

    public override void Use(HPAndBuffData hpAndBuffData)
    {
        hpAndBuffData.AddBuff(new BuffDataStrength
        {
            StackInfo = new BuffStackInfo {Count = new Observable<int>( -StackInfo!.Count.Value)}
        });
    }
}