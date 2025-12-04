using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(22)][Serializable]
public class Card22(CardData parent) : CardInTurn(parent)
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.HandList
            .Where(handCard => handCard.Parent.Config.Category != ECardCategory.Attack)
            .ToList()
            .ForEach(handCard =>
            {
                bothTurnData.HandList.MyRemove(handCard);
                bothTurnData.ExhaustList.MyAdd(handCard);
            });
        return UniTask.CompletedTask;
    }
}