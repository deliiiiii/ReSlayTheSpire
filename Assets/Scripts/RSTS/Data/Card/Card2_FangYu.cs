using System;

namespace RSTS;
[CardID(2)][Serializable]
public class Card2 : CardDataBase
{
    public override void OnCreate()
    {
        
    }
    int blockValue => EmbedInt(0);
    public override void Yield(BothTurnData bothTurnData)
    {
        bothTurnData.PlayerAddBlock(blockValue);
    }
}