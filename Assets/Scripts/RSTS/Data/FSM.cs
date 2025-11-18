using Newtonsoft.Json;

namespace RSTS;
public enum EGameState
{
    ChoosePlayer,
    Title,
    Battle,
}

public static class FSM
{
    public static readonly GameFSM Game = new();
}

public class GameFSM : MyFSMForData<GameFSM, EGameState, GameData>
{
    public override void Save()
    {
        base.Save();
        Battle.Save();
    }

    protected override void OnLoadArg(GameFSM load)
    {
        base.OnLoadArg(load);
        Battle = load.Battle;
        Battle.IsRegistered = false;
    }

    public override string SavePreName => "DataSaved";
    public override string SaveFileName => nameof(GameFSM);
    protected override EGameState DefaultState => EGameState.Title;

    [JsonIgnore]
    public BattleFSM Battle = new();
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

public class BattleFSM : MyFSMForData<BattleFSM, EBattleState, BattleData>
{
    public override void Save()
    {
        base.Save();
        BothTurn.Save();
    }

    protected override void OnLoadArg(BattleFSM load)
    {
        base.OnLoadArg(load);
        BothTurn = load.BothTurn;
        BothTurn.IsRegistered = false;
    }
    
    public override string SavePreName => "DataSaved";
    public override string SaveFileName => nameof(BattleFSM);

    public BothTurnFSM BothTurn = new();
    
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

public class BothTurnFSM : MyFSMForData<BothTurnFSM, EBothTurn, BothTurnData>
{
    public override string SavePreName => "DataSaved";
    public override string SaveFileName => nameof(BothTurnFSM);

    public readonly YieldCardFSM YieldCard = new();
}

public enum EYieldCardState
{
    None,
    Drag,
}

public class YieldCardFSM : MyFSMForData<YieldCardFSM, EYieldCardState, YieldCardData>
{
    public override string SavePreName => "DataSaved";
    public override string SaveFileName => nameof(YieldCardFSM);
}