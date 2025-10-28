using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(5003)][Serializable]
public class Card5003 : CardDataBase
{
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        return UniTask.CompletedTask;
    }
}
