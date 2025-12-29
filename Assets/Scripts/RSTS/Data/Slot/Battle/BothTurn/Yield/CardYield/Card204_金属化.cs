using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(204)][Serializable]
public class Card204 : Card
{
    BuffDataMetallicize Buff => BuffAt<BuffDataMetallicize>(0);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AddBuffToPlayer(Buff);
        return UniTask.CompletedTask;
    }
}