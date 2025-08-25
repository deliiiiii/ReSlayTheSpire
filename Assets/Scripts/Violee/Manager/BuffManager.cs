using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Violee;

public interface IMayHasConBuff
{
    public bool HasConBuff { get; }
    public Observable<bool> ConBuffActivated { get; }
    public ConsistentBuffData ConBuffData { get; }
}

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


    public static void AddWinBuff(string des, Action act)
    {
        var added = new WindowBuffData
        {
            Des = des,
            BuffEffect = act,
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
    
    public static void RefreshConBuffs(IEnumerable<IMayHasConBuff> items)
    {
        conBuffList.MyClear();
        
        items.ForEach(i =>
        {
            if(!i.HasConBuff || !i.ConBuffActivated)
                return;
            if (i.ConBuffData.HasCount && i.ConBuffData.Count <= 0)
                return;
            i.ConBuffData.Count.OnValueChangedAfter += _ => RefreshConBuffs(items);
            conBuffList.MyAdd(i.ConBuffData);
        });
    }
    public static bool IsWithRecordPlayer => ContainsConBuff(EConBuffType.PlayRecord);
    public static bool IsWithLamp => ContainsConBuff(EConBuffType.Lamp);
    public static bool IsWithSmallLamp => ContainsConBuff(EConBuffType.SmallLamp);
    public static bool IsWithCooler => ContainsConBuff(EConBuffType.Cooler);
    public static bool IsWithFridge => ContainsConBuff(EConBuffType.Fridge);
}

