using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RSTS.CDMV;
using UnityEngine;

namespace RSTS;

[Serializable]
public class CardInBattle : DataBaseConfig<CardInBattle, CardConfigMulti>
{
    public int UpgradeLevel;
    // public void Upgrade()
    // {
    //     if (!CanUpgrade())
    //         return; 
    //     UpgradeLevel++;
    //     // OnUpgrade?.Invoke();
    // }
    public CardUpgradeInfo CurUpgradeInfo => Config.Upgrades[UpgradeLevel];
    // public virtual bool CanUpgrade()
    // {
    //     return UpgradeLevel < Config.Upgrades.Count - 1;
    // }

    public bool ContainsKeyword(ECardKeyword keyword) => CurUpgradeInfo.Keywords.Contains(keyword);
    public CardCostBase CurCostInfo => CurUpgradeInfo.CostInfo;
    public EmbedString CurDes => CurUpgradeInfo.Des;
    public bool HasTarget => Config.HasTarget;
    public T NthEmbedAs<T>(int id)
        where T : EmbedType
        => (CurUpgradeInfo.Des.EmbedTypes.ToList()[id] as T)!;

    public TBuff NthEmbedAsBuffCopy<TBuff>(int id)
        where TBuff : BuffDataBase
        => ((CurUpgradeInfo.Des.EmbedTypes.ToList()[id] as EmbedAddBuff)!.BuffData as TBuff)!.DeepCopy();
}

[Serializable]
public abstract class CardInTurn : DataBaseAttr<CardInTurn, CardIDAttribute, CardInBattle>
{
    // 反射初始化
    [SerializeField] CardInBattle cardInBattle = null!;
    // 无来源卡牌
    public CardInTurn CreateBlindCard(int id) => CreateByAttr(id, CardInBattle.CreateByConfig(id));
    
    public CardConfigMulti Config => cardInBattle.Config;
    public CardUpgradeInfo CurUpgradeInfo => cardInBattle.CurUpgradeInfo;
    public bool ContainsKeyword(ECardKeyword keyword) => cardInBattle.ContainsKeyword(keyword);
    public CardCostBase CurCostInfo => cardInBattle.CurCostInfo;
    public EmbedString CurDes => cardInBattle.CurDes;
    public bool HasTarget => cardInBattle.HasTarget;
    public T NthEmbedAs<T>(int id) where T : EmbedType => cardInBattle.NthEmbedAs<T>(id);
    public TBuff NthEmbedAsBuffCopy<TBuff>(int id) where TBuff : BuffDataBase => cardInBattle.NthEmbedAsBuffCopy<TBuff>(id);
    
    public int TempUpgradeLevel;
    public EnemyDataBase? Target;
    // 临时加入的，如“愤怒”
    public bool IsTemporary;

    protected override void ReadContext(CardInBattle context)
    {
        cardInBattle = context;
        TempUpgradeLevel = context.UpgradeLevel;
    }

    public event Action? OnTempUpgrade;
    
    public virtual List<AttackModifyBase> GetModifyList(BothTurnData bothTurnData) => [];
    public virtual bool YieldCondition(BothTurnData bothTurnData, out string failReason)
    {
        failReason = string.Empty;
        return true;
    }
    public virtual void OnEnterBothTurn() { }
    public virtual void OnExitBothTurn() { }
    public virtual void OnPlayerTurnEnd(BothTurnData bothTurnData){}
    // public virtual bool RecommendYield(BothTurnData bothTurnData) => false;
    public bool CanUpgrade() => TempUpgradeLevel < cardInBattle.Config.Upgrades.Count - 1;

    public void UpgradeTemp()
    {
        TempUpgradeLevel++;
        OnTempUpgrade?.Invoke();
    }
    public T GetModify<T>(BothTurnData bothTurnData)
        where T : AttackModifyBase
    {
        return GetModifyList(bothTurnData).OfType<T>().FirstOrDefault()!;
    }
    public abstract UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy);
}


[AttributeUsage(AttributeTargets.Class)]
public class CardIDAttribute(int id) : IDAttribute(id);