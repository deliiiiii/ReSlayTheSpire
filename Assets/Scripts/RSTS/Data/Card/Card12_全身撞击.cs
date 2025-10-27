using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(12)][Serializable]
public class Card12 : CardDataBase
{
    public int AtkByBlock(BothTurnData bothTurnData) => bothTurnData.PlayerBlock;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, AtkByBlock(bothTurnData));
        return UniTask.CompletedTask;
    }
}
