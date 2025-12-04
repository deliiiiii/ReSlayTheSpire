using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(203)][Serializable]
public class Card203(CardData parent) : CardInTurn(parent)
{
    BuffDataStrength buffStrength => NthEmbedAsBuffCopy<BuffDataStrength>(0);
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AddBuffToPlayer(buffStrength);
        return UniTask.CompletedTask;
    }
}