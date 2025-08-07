using System;
using System.Collections.Generic;
using System.Linq;

namespace Violee;

public class BuffManager : SingletonCS<BuffManager>
{
    static readonly MyList<BuffData> buffList = [];

    static BuffManager()
    {
        buffList.OnAdd += b =>
        {
            MyDebug.Log($"Add Buff {b.GetDes()}");
            if (b is WindowBuffData winB)
            {
                var ret = GameManager.CreateAndAddBuffWindow($"{b.GetDes()}");
                ret.OnRemove(() => winB.BuffEffect());
            }
            else if (b is ConsistentBuffData conB)
            {
                OnAddConBuff?.Invoke(conB);
            }
        };
        
        // buffList.OnClear += () =>


        GameManager.PlayingState.OnEnter(() =>
        {
            OnClearAllBuff?.Invoke();
            buffList.MyClear();
        });
    }
    

    public static void WindowWatchingOClock(int hour)
    {
        int energy = hour % 2 == 0 ? 2 : 1;
        var added = new WindowBuffData
        {
            GetDes = () => $"叮! 时间到了{hour}点整...!\n鉴于你凝思了许久，精力+{energy}点。",
            BuffEffect = () =>
            {
                PlayerManager.EnergyCount.Value += energy;
            },
        };
        buffList.MyAdd(added);
    }

    public static void AddConBuff(EBuffType buffType, Func<string> getDes)
    {
        var added = new ConsistentBuffData()
        {
            GetDes = getDes,
            BuffType = buffType,
        };
        buffList.MyAdd(added);
    }

    public static event Action<ConsistentBuffData>? OnAddConBuff;
    public static event Action? OnClearAllBuff;
    public static bool ContainsBuff(EBuffType buffType) 
        => buffList.Any(b => b is ConsistentBuffData conB && conB.BuffType == buffType);
}