using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(19)][Serializable]
public class Card19 : CardInTurn
{
    int Atk => AtkAt(0);
    int AddPerUse => MiscAt(1);
    int useTime;
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk, ModifyList);
        useTime++;
        return UniTask.CompletedTask;
    }
    
    protected override List<AttackModifyBase> ModifyList
        => [new AttackModifyCard19() { BaseAtkAddByUse = AddPerUse * useTime }];
}

