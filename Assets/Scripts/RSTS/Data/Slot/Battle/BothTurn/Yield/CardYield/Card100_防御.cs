using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(100)][Serializable]
public class Card100 : Card
{
    int BlockValue => BlockAt(0);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.GainBlock(BlockValue);
        return UniTask.CompletedTask;
    }
}