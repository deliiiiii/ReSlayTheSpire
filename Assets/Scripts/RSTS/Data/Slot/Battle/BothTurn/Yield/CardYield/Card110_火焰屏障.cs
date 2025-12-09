using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(110)][Serializable]
public class Card110 : CardInTurn
{
    int Block => BlockAt(0);
    BuffFlameBarrier BuffFlameBarrier => BuffAt<BuffFlameBarrier>(1);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.GainBlock(Block);
        BothTurn.AddBuffToPlayer(BuffFlameBarrier);
        return UniTask.CompletedTask;
    }
}