using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(3)][Serializable]
public class Card3 : CardDataBase
{
    int atk => EmbedInt(0);
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.OpenDiscardOnceClick(cardData =>
        {
            bothTurnData.DiscardList.MyRemove(cardData);
            bothTurnData.DrawList.MyAdd(cardData);
        });
        return UniTask.CompletedTask;
    }
}