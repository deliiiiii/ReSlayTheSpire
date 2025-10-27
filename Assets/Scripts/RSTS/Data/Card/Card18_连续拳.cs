using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(18)][Serializable]
public class Card18 : CardDataBase
{
    int atk => EmbedInt(0);
    int time => EmbedInt(1);
    public override async UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        await bothTurnData.AttackEnemyMultiTimesAsync(Target, atk, time);
    }
}
