using System;

namespace RSTS;
[CardID(3)][Serializable]
public class Card3 : CardDataBase
{
    public override bool HasTarget => false;
    public override void Yield(BothTurnData bothTurnData){}
}