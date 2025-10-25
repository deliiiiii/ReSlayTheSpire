using System;

namespace RSTS;
[CardID(203)][Serializable]
public class Card203 : CardDataBase
{
    public override void OnCreateAddCom()
    {
        
    }

    int strength => EmbedInt(0);
    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AddBuffToPlayer(new BuffDataStrength(strength));
    }
}