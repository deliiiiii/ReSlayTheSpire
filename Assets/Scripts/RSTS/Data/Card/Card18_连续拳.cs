using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(18)][Serializable]
public class Card18 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    int time => NthEmbedAs<EmbedMisc>(1).MiscValue;
    public override async UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        await bothTurnData.AttackEnemyMultiTimesAsync(Target, atk, time);
    }
}
