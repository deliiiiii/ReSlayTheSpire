using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(5000)][Serializable]
public class Card5000 : CardInTurn
{
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        return UniTask.CompletedTask;
    }
}
