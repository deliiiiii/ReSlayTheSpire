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
    BothTurn,
    SelectReward,
    Lose,
    Win,
}

public enum EBothTurn
{
    GrossStart,
    PlayerTurnStart,
    PlayerDraw,
    PlayerYieldCard,
    PlayerTurnEnd,
    EnemyTurnStart,
    EnemyAction,
    EnemyTurnEnd,
}

public enum EYieldCardState
{
    None,
    Drag,
    Using,
}