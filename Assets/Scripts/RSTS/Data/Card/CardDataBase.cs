using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RSTS.CDMV;
using Sirenix.Utilities;
using UnityEngine;

namespace RSTS;

[Serializable]
public abstract class CardDataBase
{
    static Dictionary<int, Func<CardDataBase>> cardDic = [];
    public static void InitCardDic()
    {
        cardDic.Clear();
        var subTypeDic = typeof(CardDataBase).Assembly.GetTypes()
            .Where(x => x.IsSubclassOf(typeof(CardDataBase))
                        && x.GetAttribute<CardIDAttribute>() != null)
            .ToDictionary(x => x.GetAttribute<CardIDAttribute>().ID);
        foreach (var config in RefPoolMulti<CardConfigMulti>.Acquire())
        {
            if (!subTypeDic.TryGetValue(config.ID, out var type))
            {
                MyDebug.LogError($"class Card{config.ID} not found");
                cardDic.Add(config.ID, () => new Card_Template{ Config = config });
                continue;
            }
            cardDic.Add(config.ID, () =>
            {
                var ins = (Activator.CreateInstance(type, args: config) as CardDataBase)!;
                return ins;
            });
        }
    }
    public static CardDataBase CreateCard(int id)
    {
        if (cardDic.TryGetValue(id, out var func))
        {
            return func();
        }
        throw new Exception($"CreateCard : ID {id} out of range");
    }

    protected CardDataBase() => Config = null!;
    protected CardDataBase(CardConfigMulti config) => Config = config;

    public CardConfigMulti Config;
    public int UpgradeLevel;

    [SerializeField] bool isTempUpgrade;
    [SerializeField] int savedUpgradeLevel;
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
    public bool HasTarget => Config.HasTarget;
    public EnemyDataBase? Target;
    public bool IsTemporary;
    #endregion

    protected T NthEmbedAs<T>(int id)
        where T : EmbedType
        => (CurUpgradeInfo.Des.EmbedTypes.ToList()[id] as T)!;

    protected TBuff NthEmbedAsBuffCopy<TBuff>(int id)
        where TBuff : BuffDataBase
        => ((CurUpgradeInfo.Des.EmbedTypes.ToList()[id] as EmbedAddBuff)!.BuffData as TBuff)!.DeepCopy();

}


[AttributeUsage(AttributeTargets.Class)]
public class CardIDAttribute(int id) : Attribute
{
    public readonly int ID = id;
}
