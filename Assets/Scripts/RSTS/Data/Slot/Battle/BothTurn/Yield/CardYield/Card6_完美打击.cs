using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(6)][Serializable]
public class Card6 : CardInTurn
{
    int Atk => AtkAt(0);
    int AddPerDaJi => MiscAt(1);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk, ModifyList);
        return UniTask.CompletedTask;
    }

    protected override List<AttackModifyBase> ModifyList
        => [new AttackModifyCard6() { BaseAtkAddByDaJi = AddPerDaJi * BothTurn.DaJiCount }];
}