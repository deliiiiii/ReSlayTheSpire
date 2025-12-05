using System;
namespace RSTS;

[Serializable]
public class GameData: FSM<GameData, EGameState>
{
    public string PlayerName = "DELI";
    public bool HasLastBuff;

    [SubState<EGameState>(EGameState.Battle)]
    BattleData battleData = null!;

    protected override void Bind(Func<EGameState, IStateForData> getState)
    {
        getState(EGameState.Battle)
            .OnEnter(() =>
            {
                battleData = new BattleData(this);
                battleData.Launch(EBattleState.SelectLastBuff);
            })
            .OnExit(() =>
            {
                battleData.Release();
                battleData = null!;
            });
    }

    protected override void UnInit()
    {
    }

    void Test()
    {
        // var gameData = FSM<EGameState>.GetData<GameData>();
    }
}