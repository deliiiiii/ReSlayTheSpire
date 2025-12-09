using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(102)][Serializable]
public class Card102 : CardInTurn
{
    BuffDataStrength AddStrength => BuffAt<BuffDataStrength>(0);
    BuffDataLoseStrength LoseStrength => BuffAt<BuffDataLoseStrength>(1);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AddBuffToPlayer(AddStrength);
        BothTurn.AddBuffToPlayer(LoseStrength);
        return UniTask.CompletedTask;
    }
}