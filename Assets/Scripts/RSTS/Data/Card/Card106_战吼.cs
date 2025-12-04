using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(106)][Serializable]
public class Card106(CardData parent) : CardInTurn(parent)
{
    int draw => NthEmbedAs<EmbedDraw>(0).DrawValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.DrawSome(draw);
        bothTurnData.OpenHandCardOnceClick(1,
            handCard => handCard != this,
            handCard =>
            {
                bothTurnData.HandList.MyRemove(handCard);
                bothTurnData.DrawList.MyAdd(handCard);
            });
        return UniTask.CompletedTask;
    }
}