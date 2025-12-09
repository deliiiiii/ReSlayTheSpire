using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(5003)][Serializable]
public class Card5003 : CardInTurn
{
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        return UniTask.CompletedTask;
    }
}
