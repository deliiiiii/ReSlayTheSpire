using System;

namespace RSTS;
[CardID(3)][Serializable]
public class Card3 : CardDataBase
{
    int atk => EmbedInt(0);
    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.OpenDiscardOnceClick(cardData =>
        {
            bothTurnData.DiscardList.MyRemove(cardData);
            bothTurnData.DrawList.MyAdd(cardData);
        });
    }
}