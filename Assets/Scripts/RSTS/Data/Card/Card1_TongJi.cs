using System;

namespace RSTS;
[CardID(1)][Serializable]
public class Card1 : CardDataBase
{
    public override void OnCreateAddCom()
    {
        AddComponent<CardHasTarget>();
    }

    int atk => EmbedInt(0);
    int vulnerableCount => EmbedInt(1);

    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.AddBuffToEnemy(Target, new BuffDataVulnerable(vulnerableCount));
    }
}