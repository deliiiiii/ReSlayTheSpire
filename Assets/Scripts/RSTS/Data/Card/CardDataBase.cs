using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using RSTS.CDMV;
using Sirenix.Utilities;

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
                var ins = (Activator.CreateInstance(type) as CardDataBase)!;
                ins.Config = config;
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
    
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
    public CardConfigMulti Config;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
    public int UpgradeLevel;
    
    public abstract UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy);
    public virtual bool YieldCondition(BothTurnData bothTurnData, out string failReason)
    {
        failReason = string.Empty;
        return true;
    }

    // public virtual bool RecommendYield(BothTurnData bothTurnData) => false;
    
    public bool CanUpgrade => UpgradeLevel < Config.Upgrades.Count - 1;
    public bool ContainsKeyword(ECardKeyword keyword) => CurUpgradeInfo.Keywords.Contains(keyword);
    public CardCostBase CurCostInfo => CurUpgradeInfo.CostInfo;

    public string CurContentWithKeywords => CurUpgradeInfo.ContentWithKeywords;
    
    #region Com
    public bool HasTarget => Config.HasTarget;
    public EnemyDataBase Target = null!;
    public bool IsTemporary;
    #endregion

    protected int EmbedInt(int id) => CurUpgradeInfo.Des.EmbedIntList[id];
    protected int EmbedEnergyInt(int id) => CurUpgradeInfo.Des.EmbedEnergyIntList[id];
    
    CardUpgradeInfo CurUpgradeInfo => Config.Upgrades[UpgradeLevel];
}


[AttributeUsage(AttributeTargets.Class)]
public class CardIDAttribute(int id) : Attribute
{
    public readonly int ID = id;
}
