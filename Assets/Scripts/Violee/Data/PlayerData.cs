using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace Violee;

[Serializable]
public class PlayerData : DataBase
{
    [NonSerialized][ShowInInspector]
    List<MiniItemData> miniItems;
    public List<char> LetterList;
    public MiniItemData Stamina;
    public MiniItemData Energy;
    public MiniItemData Gloves;
    public MiniItemData Dice;
    
    public PlayerData()
    {
        miniItems = Configer.MiniItemConfigList.MiniItemConfigs.Select(x => new MiniItemData(x)).ToList();
        LetterList = [];
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