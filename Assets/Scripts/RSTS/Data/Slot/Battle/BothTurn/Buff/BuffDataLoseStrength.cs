using System;

namespace RSTS;

[Serializable]
public class BuffDataLoseStrength : BuffDataBase
{
    public override string Name => "活动肌肉";
    public override EBuffUseTime UseTime => EBuffUseTime.TurnEnd;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.TurnEnd;
    protected override EBuffDisposeType DisposeType => EBuffDisposeType.AllStack;
    public override bool HasStack => true;

    public override void Use(HPAndBuffData hpAndBuffData)
    {
        hpAndBuffData.AddBuff(new BuffDataStrength
        {
            StackCount = new Observable<int>(-StackCount),
        });
    }
}