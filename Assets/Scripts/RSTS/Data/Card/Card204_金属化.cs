using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(204)][Serializable]
public class Card204 : CardInTurn
{
    BuffDataMetallicize buff => NthEmbedAsBuffCopy<BuffDataMetallicize>(0);
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AddBuffToPlayer(buff);
        return UniTask.CompletedTask;
    }
}