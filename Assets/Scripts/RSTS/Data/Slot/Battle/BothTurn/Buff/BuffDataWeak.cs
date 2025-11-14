using System;

namespace RSTS;
[Serializable]
public class BuffDataWeak : BuffDataBase, IBuffFromAtkFinalMul
{
    public override string Name => "虚弱";
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.TurnEnd;
    protected override EBuffDisposeType DisposeType => EBuffDisposeType.OneStack;
    public override bool HasStack => true;

    public float GetAtkFinalMulti()
    {
        return -0.25f;
    }
}