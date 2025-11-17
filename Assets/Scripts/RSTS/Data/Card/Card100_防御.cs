using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(100)][Serializable]
public class Card100 : CardInTurn
{
    int blockValue => NthEmbedAs<EmbedBlock>(0).BlockValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.GainBlock(blockValue);
        return UniTask.CompletedTask;
    }
}