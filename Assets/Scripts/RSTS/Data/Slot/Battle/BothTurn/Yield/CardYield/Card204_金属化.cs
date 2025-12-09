using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(204)][Serializable]
public class Card204 : CardInTurn
{
    BuffDataMetallicize Buff => BuffAt<BuffDataMetallicize>(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AddBuffToPlayer(Buff);
        return UniTask.CompletedTask;
    }
}