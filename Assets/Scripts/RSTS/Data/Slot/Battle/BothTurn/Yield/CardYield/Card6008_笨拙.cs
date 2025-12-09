using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(6008)][Serializable]
public class Card6008 : CardInTurn
{
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        return UniTask.CompletedTask;
    }
}
