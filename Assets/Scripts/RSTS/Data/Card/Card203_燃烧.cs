using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(203)][Serializable]
public class Card203 : CardDataBase
{
    int strength => EmbedInt(0);
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AddBuffToPlayer(new BuffDataStrength(strength));
        return UniTask.CompletedTask;
    }
}