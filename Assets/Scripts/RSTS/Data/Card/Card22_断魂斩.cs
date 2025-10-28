using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;

namespace RSTS;
[CardID(22)][Serializable]
public class Card22 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.ExhaustHandCardBy(card => card.Config.Category != ECardCategory.Attack)
            .ForEach(bothTurnData.ExhaustOne);
        return UniTask.CompletedTask;
    }
}