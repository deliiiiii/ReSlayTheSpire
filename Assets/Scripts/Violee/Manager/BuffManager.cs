using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

namespace Violee;

public class BuffManager : SingletonCS<BuffManager>
{
    static readonly MyList<WindowBuffData> winBuffList = [];
    static readonly MyList<ConsistentBuffData> conBuffList = [];

    static BuffManager()
    {
        winBuffList.OnAdd += b => OnAddWindowBuff?.Invoke(b);
        conBuffList.OnAdd += b => OnAddConBuff?.Invoke(b);
        conBuffList.OnRemove += b => OnRemoveConBuff?.Invoke(b);
    }


    public static void AddWinBuffClock(int hour)
    {
        int energy = hour % 2 == 0 ? 2 : 1;
        var added = new WindowBuffData
        {
            GetDes = () => $"叮! 时间到了{hour}点整...!\n鉴于你凝思了许久，精力+{energy}点。",
            BuffEffect = () =>
            {
                // TODO 消除耦合
                MainItemMono.EnergyCount.Value += energy;
            },
        };
        winBuffList.MyAdd(added);
    }

    public static void AddConBuff(EConBuffType conBuffType, Func<string> getDes)
    {
        var added = new ConsistentBuffData()
        {
            GetDes = getDes,
            ConBuffType = conBuffType,
        };
        conBuffList.MyAdd(added);
    }

    public static event Action<WindowBuffData>? OnAddWindowBuff;
    public static event Action<ConsistentBuffData>? OnAddConBuff;
    public static event Action<ConsistentBuffData>? OnRemoveConBuff;
    
    
    static bool ContainsConBuff(EConBuffType conBuffType) 
        => conBuffList.Any(b => b.ConBuffType == conBuffType);

    public static void RefreshConBuffs(IEnumerable<SceneItemData> items)
    {
        conBuffList.MyClear();
        items.ForEach(i =>
        {
            if(i is not IHasConBuff { Activated: true} iHasConBuff)
                return;
            AddConBuff(iHasConBuff.conBuffType, iHasConBuff.GetDes);
        });
    }
    public static bool IsWithRecordPlayer => ContainsConBuff(EConBuffType.PlayRecord);
}

public interface IHasConBuff
{
    public EConBuffType conBuffType { get; }
    public string GetDes();
    public bool Activated { get; set; }
}