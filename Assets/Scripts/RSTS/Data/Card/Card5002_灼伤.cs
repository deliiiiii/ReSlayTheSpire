using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(5002)][Serializable]
public class Card5002 : CardInTurn
{
    int atk => NthEmbedAs<EmbedMisc>(0).MiscValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        return UniTask.CompletedTask;
    }

    public override void OnPlayerTurnEnd(BothTurnData bothTurnData)
    {
        bothTurnData.BurnPlayer(atk);
    }
}
