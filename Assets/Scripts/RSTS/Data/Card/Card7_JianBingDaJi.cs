using System;

namespace RSTS;
[CardID(7)][Serializable]
public class Card7 : CardDataBase
{
    int atk => EmbedInt(0);
    int draw => EmbedInt(1);
    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.DrawSome(draw);
    }
}
