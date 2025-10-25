using System;

namespace RSTS;
[CardID(0)][Serializable]
public class Card0 : CardDataBase
{
    public override void OnCreateAddCom()
    {
        AddComponent<CardHasTarget>();
    }
    int atk => EmbedInt(0);

    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
    }
}