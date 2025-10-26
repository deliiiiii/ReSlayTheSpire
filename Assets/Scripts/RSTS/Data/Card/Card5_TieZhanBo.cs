using System;

namespace RSTS;
[CardID(5)][Serializable]
public class Card5 : CardDataBase
{
    int block => EmbedInt(0);
    int atk => EmbedInt(1);
    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AddBlockToPlayer(block);
        bothTurnData.AttackEnemy(Target, atk);
    }
}