using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RSTS;

namespace RSTS;

[Card(4)]
[Serializable]
public class Card4 : Card
{
    int Atk => AtkAt(0);
    public int StrengthMulti => MiscAt(1);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk, GetModifyList(bothTurn));
        return UniTask.CompletedTask;
    }

    protected override List<AttackModifyBase> GetModifyList(BothTurn bothTurn) => [new AttackModifyCard4() { StrengthMulti = StrengthMulti }];
}