using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RSTS.CDMV;
using Sirenix.OdinInspector;
using UnityEngine;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;
[CreateAssetMenu(menuName = "RSTS/Card", order = 0)][PublicAPI][Serializable]
public class CardConfigMulti : ConfigMulti<CardConfigMulti>
{
    public ECardColor Color;
    public ECardRarity Rarity;
    public ECardCategory Category;
    [ValidateInput(nameof(CheckUpgradeCount), "如果不是状态牌或诅咒牌，升级列表不能为空")]
    public List<CardUpgradeInfo> Upgrades = [];
    
    protected override string PrefixName => "Card";
    protected override bool CheckAll()
    {
        return base.CheckAll() && CheckUpgradeCount();
    }

    [NonSerialized][ShowInInspector] bool quickState;
    [NonSerialized][ShowInInspector] bool quickCurse;
    bool CheckUpgradeCount()
    {
        if (Category is ECardCategory.State or ECardCategory.Curse)
            return true;
        return Upgrades.Count > 0;
    }
    void OnValidate()
    {
        if (quickState)
        {
            Category = ECardCategory.State;
            Color = ECardColor.None;
            Rarity = ECardRarity.Special;
            quickState = false;
            Upgrades.Clear();
        }
        if (quickCurse)
        {
            Category = ECardCategory.Curse;
            Color = ECardColor.Curse;
            Rarity = ECardRarity.Special;
            quickCurse = false;
            Upgrades.Clear();
        }
    }
}

#region Cost
public abstract class CardCostBase;
[Serializable][PublicAPI]
public class CardCostNumber : CardCostBase
{
    public int Cost;
}
[Serializable]
public class CardCostX : CardCostBase;
[Serializable]
public class CardCostNone : CardCostBase;
#endregion

#region Upgrade
[Serializable][PublicAPI]
public class CardUpgradeInfo
{
    public string Des;
    [SerializeReference]
    public CardCostBase CostInfo = new CardCostNumber();
}
#endregion