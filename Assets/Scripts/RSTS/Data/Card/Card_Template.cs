using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(-1)][Serializable]
public class Card_Template : CardDataBase
{
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        return UniTask.CompletedTask;
    }
}
