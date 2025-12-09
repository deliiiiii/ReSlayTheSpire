using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(5004)][Serializable]
public class Card5004 : CardInTurn
{
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        return UniTask.CompletedTask;
    }
}
