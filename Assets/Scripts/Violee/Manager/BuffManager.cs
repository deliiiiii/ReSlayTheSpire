using System.Collections.Generic;

namespace Violee;

public class BuffManager : SingletonCS<BuffManager>
{
    static readonly List<BuffData> buffList = [];

    public static BuffData WatchingOClock(int hour)
    {
        int added = hour % 2 == 0 ? 2 : 1;
        return new BuffData
        {
            Des = $"叮! 时间到了{hour}点整...!\n鉴于你凝思了许久，精力+{added}点。",
            BuffEffect = () =>
            {
                PlayerManager.EnergyCount.Value += added;
            },
        };
    }

    public static void AddBuff(BuffData buff)
    {
        MyDebug.Log($"Add Buff {buff.Des}");
        buffList.Add(buff);
        GameManager.CreateAndAddBuffWindow($"{buff.Des}");
    }
}