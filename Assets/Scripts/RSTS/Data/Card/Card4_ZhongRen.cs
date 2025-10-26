using System;

namespace RSTS;
[CardID(4)][Serializable]
public class Card4 : CardDataBase
{
    int atk => EmbedInt(0);
    int strengthMulti => EmbedInt(1);

    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemyWithStrengthMulti(Target, atk, strengthMulti);
    }
}