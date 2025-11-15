using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using RSTS.CDMV;

namespace RSTS;

[Serializable]
public abstract class CardDataBase : DataBaseMulti<CardDataBase, CardIDAttribute, CardConfigMulti>
{
    public int UpgradeLevel;
    
    [NonSerialized] public Action? OnUpgrade;
    
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


    public virtual void OnExitBothTurn()
    {
        if (isTempUpgrade)
        {
            isTempUpgrade = false;
            UpgradeLevel = savedUpgradeLevel;
        }
    }
    public virtual void OnPlayerTurnEnd(BothTurnData bothTurnData){}

    // public virtual bool RecommendYield(BothTurnData bothTurnData) => false;
    
    public void Upgrade(bool isTemp)
    {
        if (!CanUpgrade)
            return;
        if (isTemp)
        {
            isTempUpgrade = true;
            savedUpgradeLevel = UpgradeLevel;
        }
        UpgradeLevel++;
        OnUpgrade?.Invoke();
    }
    public CardUpgradeInfo CurUpgradeInfo => Config.Upgrades[UpgradeLevel];
    public bool CanUpgrade => UpgradeLevel < Config.Upgrades.Count - 1;
    public bool ContainsKeyword(ECardKeyword keyword) => CurUpgradeInfo.Keywords.Contains(keyword);
    public CardCostBase CurCostInfo => CurUpgradeInfo.CostInfo;
    public EmbedString CurDes => CurUpgradeInfo.Des;
    
    #region Com
    // TODO 改为组件模式吧。这样对于肉鸽还有点小巧思在里头的。
    public bool HasTarget => Config.HasTarget;
    public EnemyDataBase? Target;
    public bool IsTemporary;
    [SerializeField] bool isTempUpgrade;
    [SerializeField] int savedUpgradeLevel;
    #endregion

    protected T NthEmbedAs<T>(int id)
        where T : EmbedType
        => (CurUpgradeInfo.Des.EmbedTypes.ToList()[id] as T)!;

    protected TBuff NthEmbedAsBuffCopy<TBuff>(int id)
        where TBuff : BuffDataBase
        => ((CurUpgradeInfo.Des.EmbedTypes.ToList()[id] as EmbedAddBuff)!.BuffData as TBuff)!.DeepCopy();

}


[AttributeUsage(AttributeTargets.Class)]
public class CardIDAttribute(int id) : IDAttribute(id);