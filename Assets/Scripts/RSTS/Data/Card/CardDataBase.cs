using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RSTS.CDMV;
using UnityEngine;

namespace RSTS;

[Serializable]
public class CardInBattle : DataConfig<CardInBattle, CardConfigMulti>
{
    public int UpgradeLevel
    {
        get => CardInTurn?.TempUpgradeLevel ?? field;
        set
        {
            if (CardInTurn != null)
                CardInTurn.TempUpgradeLevel = value;
            else
                field = value;
        }
    }

    public void Upgrade()
    {
        if (!CanUpgrade())
            return; 
        UpgradeLevel++;
        // OnUpgrade?.Invoke();
    }
    public CardUpgradeInfo CurUpgradeInfo => Config.Upgrades[UpgradeLevel];
    public bool CanUpgrade() => UpgradeLevel < Config.Upgrades.Count - 1;
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
    
    [SubState<EBattleState>(EBattleState.BothTurn)]
    public CardInTurn? CardInTurn;
    
}

[Serializable]
public abstract class CardInTurn : 
    DataAttr<CardInTurn, CardIDAttribute, CardInBattle>, IBelong<CardInBattle>
{
    public static implicit operator CardInBattle(CardInTurn self) => self.Parent;
    
    // 反射初始化
    public CardInBattle Parent { get; private set; } = null!;
    
    // 无来源卡牌
    public CardInTurn CreateBlindCard(int id) => CreateByAttr(id, Parent);
    
    public CardConfigMulti Config => Parent.Config;
    public CardUpgradeInfo CurUpgradeInfo => Parent.CurUpgradeInfo;
    public bool ContainsKeyword(ECardKeyword keyword) => Parent.ContainsKeyword(keyword);
    public CardCostBase CurCostInfo => Parent.CurCostInfo;
    public EmbedString CurDes => Parent.CurDes;
    public bool HasTarget => Parent.HasTarget;
    public T NthEmbedAs<T>(int id) where T : EmbedType => Parent.NthEmbedAs<T>(id);
    public TBuff NthEmbedAsBuffCopy<TBuff>(int id) where TBuff : BuffDataBase => Parent.NthEmbedAsBuffCopy<TBuff>(id);
    
    public int TempUpgradeLevel;
    public EnemyDataBase? Target;
    // 临时加入的，如“愤怒”
    public bool IsTemporary;

    protected override void ReadContext(CardInBattle context)
    {
        Parent = context;
        Parent.CardInTurn = this;
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
    public bool CanUpgrade() => Parent.CanUpgrade();

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