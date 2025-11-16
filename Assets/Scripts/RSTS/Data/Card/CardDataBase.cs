using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using RSTS.CDMV;
using Sirenix.OdinInspector;

namespace RSTS;

[Serializable]
public abstract class CardDataBase : DataBaseMulti<CardDataBase, CardIDAttribute, CardConfigMulti>
{
    [JsonProperty] int upgradeLevel;
    public int UpgradeLevel => BothTurnCom.HasCom<CardComTemporary>(out var com) ? com.TempUpgradeLevel : upgradeLevel;

    public event Action? OnUpgrade;
    
    public abstract UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy);
    public T GetModify<T>(BothTurnData bothTurnData)
        where T : AttackModifyBase
    {
        return GetModifyList(bothTurnData).OfType<T>().FirstOrDefault()!;
    }
    public virtual List<AttackModifyBase> GetModifyList(BothTurnData bothTurnData) => [];
    public virtual bool YieldCondition(BothTurnData bothTurnData, out string failReason)
    {
        failReason = string.Empty;
        return true;
    }

    public virtual void OnEnterBothTurn()
    {
        BothTurnCom = new(this);
        if (Config.HasTarget)
        {
            BothTurnCom.TryAddCom<CardComTarget>(out _);
        }
    }

    public virtual void OnExitBothTurn()
    {
        BothTurnCom.Clear();
    }
    public virtual void OnPlayerTurnEnd(BothTurnData bothTurnData){}

    // public virtual bool RecommendYield(BothTurnData bothTurnData) => false;
    
    public void Upgrade(bool isTemp)
    {
        if (isTemp)
        {
            BothTurnCom.TryAddCom<CardComTemporary>(out var com);
            if (!CanUpgrade())
                return;
            com.TempUpgradeLevel++;
        }
        else
        {
            if (!CanUpgrade())
                return; 
            upgradeLevel++;
        }
        OnUpgrade?.Invoke();
    }
    public CardUpgradeInfo CurUpgradeInfo => Config.Upgrades[UpgradeLevel];
    public bool CanUpgrade()
    {
        if (BothTurnCom.HasCom<CardComTemporary>(out var com))
        {
            return com.TempUpgradeLevel < Config.Upgrades.Count - 1;
        }
        return UpgradeLevel < Config.Upgrades.Count - 1;
    }

    public bool ContainsKeyword(ECardKeyword keyword) => CurUpgradeInfo.Keywords.Contains(keyword);
    public CardCostBase CurCostInfo => CurUpgradeInfo.CostInfo;
    public EmbedString CurDes => CurUpgradeInfo.Des;
    
    #region Com

    public ComHolder<CardDataBase, CardComBase> BothTurnCom = null!;

    public bool HasTarget => BothTurnCom.HasCom<CardComTarget>(out _);
    public EnemyDataBase? Target
    {
        get => BothTurnCom.GetCom<CardComTarget>()?.Target;
        set
        {
            if(BothTurnCom.HasCom<CardComTarget>(out var com))
                com.Target = value;
        }
    }
    #endregion

    protected T NthEmbedAs<T>(int id)
        where T : EmbedType
        => (CurUpgradeInfo.Des.EmbedTypes.ToList()[id] as T)!;

    protected TBuff NthEmbedAsBuffCopy<TBuff>(int id)
        where TBuff : BuffDataBase
        => ((CurUpgradeInfo.Des.EmbedTypes.ToList()[id] as EmbedAddBuff)!.BuffData as TBuff)!.DeepCopy();

}

[Serializable]
public class ComHolder<TOwner, TCom>(TOwner owner)
    where TCom : ComponentBase<TOwner>
{
    [SerializeField]
    TOwner owner = owner;
    [SerializeField]
    List<TCom> comList = [];
    
    public bool TryAddCom<TSubCom>(out TSubCom com) where TSubCom : TCom, new()
    {
        com = null!;
        if (HasCom<TSubCom>(out _))
            return false;
        com = new TSubCom();
        com.OnCtor(owner);
        comList.Add(com);
        return true;
    }
    
    public bool HasCom<TSubCom>(out TSubCom com) where TSubCom : TCom, new()
    {
        return (com = comList.OfType<TSubCom>().FirstOrDefault()!) != null;
    }
    public TSubCom? GetCom<TSubCom>() where TSubCom : TCom, new()
    {
        return comList.OfType<TSubCom>().FirstOrDefault();
    }
    public void RemoveCom<TSubCom>() where TSubCom : TCom, new()
    {
        comList.RemoveAll(x => x is TSubCom);
    }
    public void Clear() => comList.Clear();
}

public abstract class ComponentBase<TOwner>
{
    public virtual void OnCtor(TOwner owner){}
}

public abstract class CardComBase : ComponentBase<CardDataBase>;

[Serializable]
public class CardComTarget : CardComBase
{
    public EnemyDataBase? Target;
}

[Serializable]
public class CardComTemporary : CardComBase
{
    public int TempUpgradeLevel;
    public override void OnCtor(CardDataBase owner)
    {
        base.OnCtor(owner);
        TempUpgradeLevel = owner.UpgradeLevel;
    }
}



[AttributeUsage(AttributeTargets.Class)]
public class CardIDAttribute(int id) : IDAttribute(id);