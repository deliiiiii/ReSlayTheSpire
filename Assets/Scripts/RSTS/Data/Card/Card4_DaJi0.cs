using System;

namespace RSTS;
[CardID(4)][Serializable]
public class Card4 : CardDataBase
{
    public override void OnCreate()
    {
        AddComponent<CardHasTarget>();
    }

    public override void Yield(BothTurnData bothTurnData)
    {
        bothTurnData.AttackEnemy(GetComponent<CardHasTarget>().Target, CurUpgradeInfo.Des.EmbedIntList[0]);
    }
}