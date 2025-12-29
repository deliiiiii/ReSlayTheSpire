using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(5000)][Serializable]
public class Card5000 : Card
{
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        return UniTask.CompletedTask;
    }
}
