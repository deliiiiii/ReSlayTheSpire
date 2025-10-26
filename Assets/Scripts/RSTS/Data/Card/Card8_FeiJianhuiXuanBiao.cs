using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(8)][Serializable]
public class Card8 : CardDataBase
{
    int atk => EmbedInt(0);
    int atkTime => EmbedInt(1);
    public override void Yield(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemyRandomlyMultiTimes(atk, atkTime).Forget();
    }
}
