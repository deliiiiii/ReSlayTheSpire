using System;

namespace RSTS;
[CardID(90)][Serializable]
public class Card90 : CardDataBase
{
    int atk => EmbedInt(0);

    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
    }
}