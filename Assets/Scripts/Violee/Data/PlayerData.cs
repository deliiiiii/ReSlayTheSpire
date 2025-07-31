using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

[Serializable]
public class PlayerData : DataBase
{
    [NonSerialized][ShowInInspector]
    List<MiniItemData> miniItems = [];
    public List<char> LetterList = [];
    public MiniItemData Stamina = null!;
    public MiniItemData Energy = null!;
    public MiniItemData Gloves = null!;
    public MiniItemData Dice = null!;
    
    public PlayerData()
    {
        // 单例在没有运行时是不能FindObjectOfType的
        // if (!Configer.Started)
        //     return;
        foreach (var config in Configer.MiniItemConfigList.MiniItemConfigs)
        {
            miniItems.Add(new MiniItemData(config));
        }
        Stamina = miniItems.Find(x => x.Config.Description.Equals(nameof(Stamina)));
        Energy = miniItems.Find(x => x.Config.Description.Equals(nameof(Energy)));
        Gloves = miniItems.Find(x => x.Config.Description.Equals(nameof(Gloves)));
        Dice = miniItems.Find(x => x.Config.Description.Equals(nameof(Dice)));
        // TODO 
        Stamina.OnRunOut += () => {/* game over */ };
    }
}



[Serializable]
public class MiniItemData(MiniItemConfig config) : DataBase
{
    public MiniItemConfig Config = config;
    public Observable<int> Count = new(config.InitValue);
    
    public event Func<bool>? CheckUse;
    public event Action<int>? OnCheckFail;
    public event Action? OnUse;
    public event Action? OnRunOut;

    public void TryUse()
    {
        if (CheckUse != null && !CheckUse())
        {
            OnCheckFail?.Invoke(Count.Value);
            return;
        }
        Use();
    }

    void Use()
    {
        Count.Value--;
        OnUse?.Invoke();
        if (Count == 0)
        {
            OnRunOut?.Invoke();
        }
    }
}

// public List<ItemCom> ComList = [];
// public ItemCom? GetCom<T>() => ComList.Find(x => x.GetType() == typeof(T));
public abstract class ItemCom;

public class ItemStackable : ItemCom
{
    public int Count;
}