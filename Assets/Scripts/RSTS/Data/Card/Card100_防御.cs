using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(100)][Serializable]
public class Card100 : CardDataBase
{
    int blockValue => NthEmbedAs<EmbedBlock>(0).BlockValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AddBlockToPlayer(blockValue);
        return UniTask.CompletedTask;
    }
}