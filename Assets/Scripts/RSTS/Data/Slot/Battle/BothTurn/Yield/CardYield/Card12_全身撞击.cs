using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(12)][Serializable]
public class Card12 : CardInTurn
{
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, 0, ModifyList);
        return UniTask.CompletedTask;
    }

    protected override List<AttackModifyBase> ModifyList
        => [new AttackModifyCard12() { AtkByBlock = BothTurn.PlayerBlock }];
}
