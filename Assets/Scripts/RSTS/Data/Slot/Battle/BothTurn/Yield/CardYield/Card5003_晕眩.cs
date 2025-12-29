using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(5003)][Serializable]
public class Card5003 : Card
{
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        return UniTask.CompletedTask;
    }
}
