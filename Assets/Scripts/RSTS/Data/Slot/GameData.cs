using System;
namespace RSTS;

public static class FSM
{
    public static readonly GameData GameData = new();
}

[Serializable]
public class GameData: FSM<GameData, EGameState>
{
    public string PlayerName = "DELI";
    public bool HasLastBuff;

    [SubState<EGameState>(EGameState.Battle)]
    BattleData battleData = null!;
    public GameData()
    {
        GetState(EGameState.Battle)
            .OnEnter(() =>
            {
                battleData = new BattleData(this);
                battleData.Launch(EBattleState.SelectLastBuff);
            })
            .OnUpdate(_ => {})
            .OnExit(() =>
            {
                battleData.Release();
                battleData = null!;
            });
    }
    protected override void UnInit()
    {
    }
}