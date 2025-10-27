using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(5004)][Serializable]
public class Card5004 : CardDataBase
{
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        return UniTask.CompletedTask;
    }
}
