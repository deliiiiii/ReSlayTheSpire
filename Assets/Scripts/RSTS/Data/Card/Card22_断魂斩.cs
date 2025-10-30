using System;
using System.Collections.Generic;
using System.Linq;
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
        bothTurnData.HandList
            .Where(handCard => handCard.Config.Category != ECardCategory.Attack)
            .ForEach(handCard =>
            {
                bothTurnData.HandList.MyRemove(handCard);
                bothTurnData.ExhaustList.MyAdd(handCard);
            });
        return UniTask.CompletedTask;
    }
}