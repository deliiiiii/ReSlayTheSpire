using System;

namespace RSTS;
[CardID(1)][Serializable]
public class Card1 : CardDataBase
{
    public override bool HasTarget() => true;

    public override void Yield(BothTurnData bothTurnData)
    {
        
    }
}