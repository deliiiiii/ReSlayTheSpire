using System;

namespace RSTS;
[CardID(100)][Serializable]
public class Card100 : CardDataBase
{
    int blockValue => EmbedInt(0);

    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AddBlockToPlayer(blockValue);
    }
}