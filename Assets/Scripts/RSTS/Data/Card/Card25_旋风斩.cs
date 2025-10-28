using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(25)][Serializable]
public class Card25 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;

    public override async UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        await bothTurnData.AttackAllEnemiesMultiTimesAsync(atk, costEnergy);
    }

}