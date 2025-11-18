using System;
using System.Diagnostics.CodeAnalysis;
using RSTS;
using Sirenix.OdinInspector;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

public class TestAnything : Singleton<TestAnything>
{
    [ShowInInspector] string GameState => FSM.Game.CurStateName;
    [ShowInInspector] static string BattleState => FSM.Game.Battle.CurStateName;
    [ShowInInspector] static string YieldCardState => FSM.Game.Battle.BothTurn.YieldCard.CurStateName;
    [Button] public void SaveGameFSM() => FSM.Game.Save();
    [Button] public void EnterGameState(EGameState gameState) => FSM.Game.EnterState(gameState);
    [Button] public void EnterBattleState(EBattleState battleState) => FSM.Game.Battle.EnterState(battleState);
    [Button] public void EnterBothTurnState(EBothTurn bothTurnState) => FSM.Game.Battle.BothTurn.EnterState(bothTurnState);

    // [field: AllowNull, MaybeNull]
    // BothTurnData BothTurnData
    // {
    //     get
    //     {
    //         if (field != null)
    //             return field;
    //         if(!FSM.Game.Battle.BothTurn.IsState(EBothTurn.PlayerYieldCard))
    //         {
    //             throw new InvalidOperationException("Not in BothTurn PlayerYieldCard state");
    //         }
    //         return field = FSM.Game.Battle.BothTurn.Arg;
    //     }
    // }
    //
    // [Button]
    // public void AttackPlayer(int baseAtk, int enemyID)
    // {
    //     if(enemyID < 0 || enemyID >= BothTurnData.EnemyList.Count)
    //         return;
    //     BothTurnData.AttackPlayerFromEnemy(BothTurnData.EnemyList[enemyID], baseAtk, out _);
    // }
    // [Button] public void ChangePlayerHP(int delta) => BothTurnData.PlayerCurHP.Value += delta;
    // [Button] public void DrawCard(int count) => BothTurnData.DrawSome(count);
}