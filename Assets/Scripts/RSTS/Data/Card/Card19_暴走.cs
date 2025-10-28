using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(19)][Serializable]
public class Card19 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    int addPerUse => NthEmbedAs<EmbedMisc>(1).MiscValue;
    int useTime;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk, GetModifyList(bothTurnData));
        useTime++;
        return UniTask.CompletedTask;
    }

    public override void OnExitBothTurn()
    {
        useTime = 0;
    }

    public override List<AttackModifyBase> GetModifyList(BothTurnData bothTurnData)
        => [new AttackModifyCard19() { BaseAtkAddByUse = addPerUse * useTime }];
}

