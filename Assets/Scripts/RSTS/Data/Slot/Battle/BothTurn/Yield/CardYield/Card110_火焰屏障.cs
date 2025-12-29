using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(110)][Serializable]
public class Card110 : Card
{
    int Block => BlockAt(0);
    BuffFlameBarrier BuffFlameBarrier => BuffAt<BuffFlameBarrier>(1);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.GainBlock(Block);
        bothTurn.AddBuffToPlayer(BuffFlameBarrier);
        return UniTask.CompletedTask;
    }
}