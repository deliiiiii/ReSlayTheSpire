using System;

namespace RSTS;
[CardID(2)][Serializable]
public class Card2 : CardDataBase
{
    int atk => EmbedInt(0);

    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        var copied = this.DeepCopy();
        bothTurnData.AddTempToDiscard(copied);
    }
}