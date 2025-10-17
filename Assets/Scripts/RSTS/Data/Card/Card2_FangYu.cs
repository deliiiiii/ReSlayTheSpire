using System;

namespace RSTS;
[CardID(2)][Serializable]
public class Card2 : CardDataBase
{
    public override bool HasTarget => false;

    public override void Yield(BothTurnData bothTurnData){}
}