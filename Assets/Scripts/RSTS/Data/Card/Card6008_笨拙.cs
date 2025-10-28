using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(6008)][Serializable]
public class Card6008 : CardDataBase
{
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        return UniTask.CompletedTask;
    }
}
