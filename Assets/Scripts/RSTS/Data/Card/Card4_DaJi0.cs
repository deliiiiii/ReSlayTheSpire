using System;

namespace RSTS;
[CardID(4)][Serializable]
public class Card4 : CardDataBase
{
    public override bool HasTarget => true;

    public override void Yield(BothTurnData bothTurnData)
    {
        bothTurnData.AttackEnemy(Target, CurUpgradeInfo.Des.EmbedIntList[0]);
    }
}