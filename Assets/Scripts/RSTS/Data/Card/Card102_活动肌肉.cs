using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(102)][Serializable]
public class Card102 : CardDataBase
{
    BuffDataStrength addStrength => NthEmbedAsBuffCopy<BuffDataStrength>(0);
    BuffDataLoseStrength loseStrength => NthEmbedAsBuffCopy<BuffDataLoseStrength>(1);

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AddBuffToPlayer(addStrength);
        bothTurnData.AddBuffToPlayer(loseStrength);
        return UniTask.CompletedTask;
    }
}