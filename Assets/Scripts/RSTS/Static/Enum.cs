namespace RSTS;

public enum EPlayerJob
{
    ZhanShi,
    LieShou,
    JiBao,
    GuanZhe
}

public enum ECardColor
{
    Red,
    Green,
    Blue,
    Purple,
    None,
    Curse,
}
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
    /// 诅咒
    Curse,
    /// 基本
    Basic
}
public enum ECardCategory
{
    /// 攻击
    Attack,
    /// 技能
    Skill,
    /// 能力：打出后获得一次性buff，且不进入消耗堆/弃牌堆
    Ability,
    /// 状态：战斗结束后移除牌组
    State,
    /// 诅咒：留在牌组中的debuff牌
    Curse,
}

public enum ECardKeyword
{
    /// 固有：第一回合必然在手牌
    Inborn,
    /// 虚无：回合结束时进入消耗堆
    Ethereal,
    /// 不可被打出
    Unplayable,
    /// 保留：回合结束时不进入弃牌堆，而是留在手牌
    Retain,
    /// 消耗：打出后进入消耗堆，本场战斗不再抽取与使用
    Exhaust,
}