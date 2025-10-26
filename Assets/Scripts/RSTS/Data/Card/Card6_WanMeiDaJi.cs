using System;

namespace RSTS;
[CardID(6)][Serializable]
public class Card6 : CardDataBase
{
    int atk => EmbedInt(0);
    int addPerDaJi => EmbedInt(1);
    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk + addPerDaJi * bothTurnData.DaJiCount);
    }
}