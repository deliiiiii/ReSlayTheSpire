using System;

namespace RSTS;

[Serializable]
public class BuffDataStrength : BuffDataBase, IBuffFromAtkBaseAdd
{
    public override string Name => "力量";
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.None;
    protected override EBuffDisposeType DisposeType => EBuffDisposeType.Never;
    public override bool HasStack => true;

    public int GetAtkBaseAdd() => StackCount;
}