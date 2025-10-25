using System;

namespace RSTS;
[Serializable]
public class BuffDataVulnerable : BuffDataBase, IBuffAtkFinalMul
{
    public override string Name => "易伤";
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.TurnEnd;
    
    public BuffDataVulnerable(int count)
    {
        StackInfo = new BuffStackInfo(count);
    }

    public float GetAtkFinalMulti()
    {
        return 0.5f;
    }
    
    public override bool Dispose()
    {
        StackInfo!.Count.Value--;
        return StackInfo.Count <= 0;
    }
}