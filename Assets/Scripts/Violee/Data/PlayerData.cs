using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Violee;

[Serializable]
public class PlayerData : DataBase
{
    [NonSerialized]
    List<MiniItemData> miniItems = [];
    public List<char> LetterList = [];
    [field: MaybeNull] public MiniItemData Stamina 
        => field ??= miniItems.Find(x => x.Config.Description.Equals(nameof(Stamina)));
    [field: MaybeNull] public MiniItemData Energy
        => field ??= miniItems.Find(x => x.Config.Description.Equals(nameof(Energy)));
    [field: MaybeNull] public MiniItemData Gloves
        => field ??= miniItems.Find(x => x.Config.Description.Equals(nameof(Gloves)));
    [field: MaybeNull] public MiniItemData Dice
        => field ??= miniItems.Find(x => x.Config.Description.Equals(nameof(Dice)));
    
    public PlayerData()
    {
        foreach (var config in Configer.MiniItemConfigList.MiniItemConfigs)
        {
            miniItems.Add(new MiniItemData(config));
        }
        
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