using System;

namespace RSTS;
[CardID(-1)][Serializable]
public class Card_Template : CardDataBase
{
    public override void OnCreateAddCom()
    {
        
    }

    public override void Yield(BothTurnData bothTurnData, int costEnergy){}
}