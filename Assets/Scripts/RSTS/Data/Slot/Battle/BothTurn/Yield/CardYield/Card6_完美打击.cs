using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(6)][Serializable]
public class Card6 : Card
{
    int Atk => AtkAt(0);
    int AddPerDaJi => MiscAt(1);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk, GetModifyList(bothTurn));
        return UniTask.CompletedTask;
    }

    protected override List<AttackModifyBase> GetModifyList(BothTurn bothTurn) =>
        [new AttackModifyCard6() { BaseAtkAddByDaJi = AddPerDaJi * bothTurn.DaJiCount }];
}