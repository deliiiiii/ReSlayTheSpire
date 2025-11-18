using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using RSTS;
using Sirenix.OdinInspector;
using UnityEngine;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

public class TestAnything : Singleton<TestAnything>
{
    [Button] public void EnterGameState(EGameState gameState) => GameFSM.EnterStateStatic(gameState);
    [Button] public void EnterBattleState(EBattleState battleState) => BattleFSM.EnterStateStatic(battleState);
    [Button] public void EnterBothTurnState(EBothTurn bothTurnState) => BothTurnFSM.EnterStateStatic(bothTurnState);

    [field: AllowNull, MaybeNull][ShowIf(nameof(ShowBothTurnData))]
    BothTurnData BothTurnData
    {
        get
        {
            if (field != null)
                return field; 
            if(!BothTurnFSM.IsStateStatic(EBothTurn.PlayerYieldCard, out var fsm))
            {
                throw new InvalidOperationException("Not in BothTurn PlayerYieldCard state");
            }
            return field = fsm.Arg;
        }
        set;
    }
    bool ShowBothTurnData => BothTurnFSM.IsStateStatic(EBothTurn.PlayerYieldCard);

    [Button]
    public void AttackPlayer(int baseAtk, int enemyID)
    {
        if(enemyID < 0 || enemyID >= BothTurnData.EnemyList.Count)
            return;
        BothTurnData.AttackPlayerFromEnemy(BothTurnData.EnemyList[enemyID], baseAtk, out _);
    }
    [Button] public void ChangePlayerHP(int delta) => BothTurnData.PlayerCurHP.Value += delta;
    [Button] public void DrawCard(int count) => BothTurnData.DrawSome(count);
    [Button] public void SaveGameData() => BothTurnData.Save();
    [Button] public void LoadGameData() => BothTurnData = BothTurnData.Load();
}