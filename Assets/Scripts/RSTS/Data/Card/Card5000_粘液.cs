using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(5000)][Serializable]
public class Card5000 : CardDataBase
{
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        return UniTask.CompletedTask;
    }
}
