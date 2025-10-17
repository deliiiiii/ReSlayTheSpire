using System;

namespace RSTS;
[CardID(-1)][Serializable]
public class Card_Template : CardDataBase
{
    public override bool HasTarget => false;
    public override void Yield(BothTurnData bothTurnData){}
}