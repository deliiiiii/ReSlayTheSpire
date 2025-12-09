using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RSTS;

namespace RSTS;

[Card(4)]
[Serializable]
public class Card4 : CardInTurn
{
    int Atk => AtkAt(0);
    public int StrengthMulti => MiscAt(1);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk, ModifyList);
        return UniTask.CompletedTask;
    }

    protected override List<AttackModifyBase> ModifyList 
        => [new AttackModifyCard4(){StrengthMulti = StrengthMulti}];
}