using System;

namespace RSTS;

[Serializable]
// ReSharper disable once InconsistentNaming
public class BuffData_Template : BuffDataBase
{
    public override string Name => "模板Buff";
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.None;
    protected override EBuffDisposeType DisposeType => EBuffDisposeType.Never;
    public override bool HasStack => false;
}