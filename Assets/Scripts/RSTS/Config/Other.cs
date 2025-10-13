using System;
using JetBrains.Annotations;
using UnityEngine;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

[PublicAPI]
public enum ECardColor
{
    Red,
    Green,
    Blue,
    Purple,
    None,
    Curse,
}

[PublicAPI]
public enum ECardRarity
{
    /// 普通or初始
    Normal,
    /// 稀有
    Rare,
    /// 罕见
    Epic,
    /// 特殊
    Special,
}

[PublicAPI]
public enum ECardCategory
{
    /// 攻击
    Attack,
    /// 技能
    Skill,
    /// 能力
    Ability,
    /// 状态
    State,
    /// 诅咒
    Curse,
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