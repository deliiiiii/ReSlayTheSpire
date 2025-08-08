using System;
using System.Collections.Generic;
using System.Linq;
using Violee;

[Serializable]
public class MainItemData : DataBase
{
    public List<MainItemDataSingle> MiniItems;
    
    public List<char> LetterList;
    
    [NonSerialized] public MainItemDataSingle Stamina;
    [NonSerialized] public MainItemDataSingle Energy;
    [NonSerialized] public MainItemDataSingle Creativity;
    [NonSerialized] public MainItemDataSingle Violee;
    
    public MainItemData(int xxx = 0)
    {
        MiniItems = Configer.mainItemConfigList.MainItemConfigs.Select(x => new MainItemDataSingle(x)).ToList();
        LetterList = [];
        Stamina = MiniItems.Find(x => x.Config.Description.Equals(nameof(Stamina)));
        Energy = MiniItems.Find(x => x.Config.Description.Equals(nameof(Energy)));
        Creativity = MiniItems.Find(x => x.Config.Description.Equals(nameof(Creativity)));
        Violee = MiniItems.Find(x => x.Config.Description.Equals(nameof(Violee)));
        // TODO 
        Stamina.OnRunOut += () => {/* game over */ };
    }
}


[Serializable]
public class MainItemDataSingle(MainItemConfig config) : DataBase
{
    public MainItemConfig Config = config;
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