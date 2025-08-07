using System.Collections.Generic;

namespace Violee;

public class BuffManager
{
    public List<BuffData> BuffList = [];
    
    public void AddBuff(BuffData buff)
    {
        BuffList.Add(buff);
    }
    public static BuffData TestBuff => new() {BuffEffect = () => MyDebug.Log("Test Buff Effect")};
}