using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

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
            Des = $"叮! 时间到了{hour}点整...!\n鉴于你凝思了许久，精力+{energy}点。",
            BuffEffect = () =>
            {
                // TODO 消除耦合
                MainItemMono.GainEnergy(energy);
            },
        };
        winBuffList.MyAdd(added);
    }
    
    public static void TryUseSmallLamp()
    {
        var smallLamp = conBuffList.Find(b => b.ConBuffType == EConBuffType.SmallLamp);
        if (smallLamp == null)
            return;
        smallLamp.Count.Value--;
    }

    public static event Action<WindowBuffData>? OnAddWindowBuff;
    public static event Action<ConsistentBuffData>? OnAddConBuff;
    public static event Action<ConsistentBuffData>? OnRemoveConBuff;
    
    
    static bool ContainsConBuff(EConBuffType conBuffType) 
        => conBuffList.Any(b => b.ConBuffType == conBuffType);

    static readonly UnityAction<int> refreshBuffActInt = _ => PlayerMono.RefreshCurPointBuff();
    
    public static void RefreshConBuffs(IEnumerable<SceneItemData> items)
    {
        conBuffList.MyClear();
        
        items.ForEach(i =>
        {
            if(!i.HasConBuff || !i.ConBuffActivated)
                return;
            if (i.ConBuffData.HasCount && i.ConBuffData.Count <= 0)
                return;
            i.ConBuffData.Count.OnValueChangedAfter += refreshBuffActInt;
            conBuffList.MyAdd(i.ConBuffData);
        });
    }
    public static bool IsWithRecordPlayer => ContainsConBuff(EConBuffType.PlayRecord);
    public static bool IsWithLamp => ContainsConBuff(EConBuffType.Lamp);
    public static bool IsWithSmallLamp => ContainsConBuff(EConBuffType.SmallLamp);
    public static bool IsWithCooler => ContainsConBuff(EConBuffType.Cooler);
}

