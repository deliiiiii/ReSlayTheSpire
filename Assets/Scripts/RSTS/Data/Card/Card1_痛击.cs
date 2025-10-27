using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(1)][Serializable]
public class Card1 : CardDataBase
{
    int atk => EmbedInt(0);
    int vulnerableCount => EmbedInt(1);

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.AddBuffToEnemy(Target, new BuffDataVulnerable(vulnerableCount));
        return UniTask.CompletedTask;
    }
}