using System.Collections.Generic;

namespace Violee;

public class BuffManager : SingletonCS<BuffManager>
{
    static readonly MyList<BuffData> buffList = [];

    static BuffManager()
    {
        buffList.OnAdd += b =>
        {
            MyDebug.Log($"Add Buff {b.Des}");
            var ret = GameManager.CreateAndAddBuffWindow($"{b.Des}");
            if (b.EffectType == EBuffEffectType.EffectOnCloseWindow)
            {
                ret.OnRemove(() => b.BuffEffect());
            }
        };
    }
    

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
            EffectType = EBuffEffectType.EffectOnCloseWindow,
        };
    }

    public static void AddBuff(BuffData buff)
    {
        buffList.MyAdd(buff);
    }
}