using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(12)][Serializable]
public class Card12 : CardDataBase
{
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, bothTurnData.PlayerBlock);
        return UniTask.CompletedTask;
    }
}
