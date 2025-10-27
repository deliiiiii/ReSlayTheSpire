using System;

namespace RSTS;
[Serializable]
public class BuffDataWeak : BuffDataBase, IBuffFromAtkFinalMul
{
    public override string Name => "虚弱";
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.TurnEnd;
    
    public BuffDataWeak(int count)
    {
        StackInfo = new BuffStackInfo(count);
    }

    public float GetAtkFinalMulti()
    {
        return -0.25f;
    }
    
    public override bool Dispose()
    {
        StackInfo!.Count.Value--;
        return StackInfo.Count <= 0;
    }
}