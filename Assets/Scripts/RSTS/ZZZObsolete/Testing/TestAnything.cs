using System;
using System.Globalization;using RSTS;
using Sirenix.OdinInspector;
using UnityEngine;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

public class TestAnything : Singleton<TestAnything>
{
    [Button]
    public void EnterGameState(EGameState gameState)
    {
        MyFSM.EnterState(GameStateWrap.One, gameState);
    }

    [Button]
    public void EnterBattleState(EBattleState battleState)
    {
        MyFSM.EnterState(BattleStateWrap.One, battleState);
    }
    [Button]
    public void EnterBothTurnState(EBothTurn bothTurnState)
    {
        MyFSM.EnterState(BothTurnStateWrap.One, bothTurnState);
    }
    
    [SerializeReference]
    public BattleData BattleData;
    [SerializeReference]
    public BothTurnData BothTurnData;

    [Button]
    public void AttackPlayer(int baseAtk, int enemyID)
    {
        BothTurnData = null!;
        if (!MyFSM.IsState(BothTurnStateWrap.One, EBothTurn.PlayerYieldCard, out var bothTurnData))
            return;
        BothTurnData = bothTurnData;
        if(enemyID < 0 || enemyID >= BothTurnData.EnemyList.Count)
            return;
        BothTurnData.AttackPlayerFromEnemy(bothTurnData.EnemyList[enemyID], baseAtk, out _);
    }
    
    [Button]
    public void ChangePlayerHP(int delta)
    {
        if (!MyFSM.IsState(BattleStateWrap.One, EBattleState.BothTurn, out var battleData))
            return;
        BattleData = battleData;
        BattleData.CurHP.Value += delta;
    }

    [Button]
    public void DrawCard(int count)
    {
        if (!MyFSM.IsState(BothTurnStateWrap.One, EBothTurn.PlayerYieldCard, out var bothTurnData))
            return;
        BothTurnData = bothTurnData;
        BothTurnData.DrawSome(count);
    }
}