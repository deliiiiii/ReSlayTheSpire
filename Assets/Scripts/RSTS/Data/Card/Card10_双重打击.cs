using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(10)][Serializable]
public class Card10 : CardDataBase
{
    int atk => EmbedInt(0);
    public override async UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        await bothTurnData.AttackEnemyMultiTimesAsync(Target, atk, 2);
    }
}
