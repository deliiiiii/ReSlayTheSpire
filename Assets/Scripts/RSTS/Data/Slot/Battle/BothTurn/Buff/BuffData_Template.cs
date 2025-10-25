using System;

namespace RSTS;

[Serializable]
public class BuffData_Template : BuffDataBase
{
    public override string Name => "模板Buff";
    public override EBuffUseTime UseTime => EBuffUseTime.None;
    public override EBuffDisposeTime DisposeTime => EBuffDisposeTime.None;
    
    public BuffData_Template(int count)
    {
        StackInfo = new BuffStackInfo(count);
    }
}