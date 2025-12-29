using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(12)][Serializable]
public class Card12 : Card
{
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, 0, GetModifyList(bothTurn));
        return UniTask.CompletedTask;
    }

    protected override List<AttackModifyBase> GetModifyList(BothTurn bothTurn) => [new AttackModifyCard12() { AtkByBlock = bothTurn.PlayerBlock }];
}
