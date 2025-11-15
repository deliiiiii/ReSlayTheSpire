using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(29)][Serializable]
public class Card29 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackAllEnemies(atk);
        bothTurnData.AddTempToDiscard(() => CreateData(5002));
        return UniTask.CompletedTask;
    }
}