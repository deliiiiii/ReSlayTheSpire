using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(14)][Serializable]
public class Card14 : CardDataBase
{
    int atk => EmbedInt(0);
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackAllEnemies(atk);
        return UniTask.CompletedTask;
    }
}
