using System;

namespace RSTS;

[Serializable]
public class BuffFromDataStrength : BuffDataBase, IBuffFromAtkBaseAdd
{
    public override string Name => "力量";
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.None;
    
    public BuffFromDataStrength(int count)
    {
        StackInfo = new BuffStackInfo(count);
    }

    public int GetAtkBaseAdd()
    {
        return StackInfo?.Count ?? 0;
    }
}