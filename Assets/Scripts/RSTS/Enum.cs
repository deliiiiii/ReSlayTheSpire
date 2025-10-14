namespace RSTS;

public enum EGameState
{
    ChoosePlayer,
    Title,
    Battle,
}
public enum EBattleState
{
    SelectLastBuff,
    SelectRoom,
    YieldCard,
    SelectReward,
    Lose,
    Win,
}

public enum EYieldCardState
{
    Start,
    PlayerDraw,
    PlayerYield,
    PlayerYieldEnd,
    PlayerRecycle,
    End,
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
}
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