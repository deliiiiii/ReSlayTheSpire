using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(9)][Serializable]
public class Card9 : CardDataBase
{
    int atk => EmbedInt(0);
    int vulnerableCount => EmbedInt(1);
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackAllEnemies(atk);
        bothTurnData.AddBuffToAllEnemies(() => new BuffDataVulnerable(vulnerableCount));
        return UniTask.CompletedTask;
    }
}
