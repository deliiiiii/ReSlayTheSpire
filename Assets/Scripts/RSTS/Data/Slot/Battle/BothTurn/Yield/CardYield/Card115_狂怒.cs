using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(115)][Serializable]
public class Card115 : Card
{
    BuffDataAttackGainBlock BuffAttackGainBlock => BuffAt<BuffDataAttackGainBlock>(0);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AddBuffToPlayer(BuffAttackGainBlock);
        return UniTask.CompletedTask;
    }
}