using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(6008)][Serializable]
public class Card6008 : Card
{
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        return UniTask.CompletedTask;
    }
}
