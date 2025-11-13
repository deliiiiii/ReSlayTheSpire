using System;

namespace RSTS;
[Serializable]
public class BuffDataVulnerable : BuffDataBase, IBuffToAtkFinalMul
{
    public override string Name => "易伤";
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.TurnEnd;
    protected override EBuffDisposeType DisposeType => EBuffDisposeType.OneStack;
    public float GetAtkFinalMulti()
    {
        return +0.5f;
    }
}