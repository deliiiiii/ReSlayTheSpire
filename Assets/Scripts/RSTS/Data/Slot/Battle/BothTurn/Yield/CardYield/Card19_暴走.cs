using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(19)][Serializable]
public class Card19 : Card
{
    int Atk => AtkAt(0);
    int AddPerUse => MiscAt(1);
    int useTime;
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk, GetModifyList(bothTurn));
        useTime++;
        return UniTask.CompletedTask;
    }

    protected override List<AttackModifyBase> GetModifyList(BothTurn bothTurn) => 
        [new AttackModifyCard19() { BaseAtkAddByUse = AddPerUse * useTime }];
}

