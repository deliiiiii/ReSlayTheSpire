using System;

namespace RSTS;

[Serializable]
public class BuffDataStrength : BuffDataBase, IBuffAtkBaseAdd
{
    public override string Name => "力量";
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.None;
    
    public BuffDataStrength(int count)
    {
        StackInfo = new BuffStackInfo(count);
    }

    public int GetAtkBaseAdd()
    {
        return StackInfo?.Count ?? 0;
    }
}