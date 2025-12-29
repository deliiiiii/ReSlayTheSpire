using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(5004)][Serializable]
public class Card5004 : Card
{
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        return UniTask.CompletedTask;
    }
}
