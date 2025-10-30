using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(110)][Serializable]
public class Card110 : CardDataBase
{
    int block => NthEmbedAs<EmbedBlock>(0).BlockValue;
    BuffFlameBarrier buffFlameBarrier => NthEmbedAsBuffCopy<BuffFlameBarrier>(1);
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.GainBlock(block);
        bothTurnData.AddBuffToPlayer(buffFlameBarrier);
        return UniTask.CompletedTask;
    }
}