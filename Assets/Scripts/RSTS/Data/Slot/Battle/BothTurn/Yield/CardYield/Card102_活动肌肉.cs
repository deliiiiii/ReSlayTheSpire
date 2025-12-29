using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(102)][Serializable]
public class Card102 : Card
{
    BuffDataStrength AddStrength => BuffAt<BuffDataStrength>(0);
    BuffDataLoseStrength LoseStrength => BuffAt<BuffDataLoseStrength>(1);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AddBuffToPlayer(AddStrength);
        bothTurn.AddBuffToPlayer(LoseStrength);
        return UniTask.CompletedTask;
    }
}