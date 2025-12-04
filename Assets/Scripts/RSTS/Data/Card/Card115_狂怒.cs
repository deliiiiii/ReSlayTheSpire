using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(115)][Serializable]
public class Card115(CardData parent) : CardInTurn(parent)
{
    BuffDataAttackGainBlock buffAttackGainBlock => NthEmbedAsBuffCopy<BuffDataAttackGainBlock>(0);
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AddBuffToPlayer(buffAttackGainBlock);
        return UniTask.CompletedTask;
    }
}