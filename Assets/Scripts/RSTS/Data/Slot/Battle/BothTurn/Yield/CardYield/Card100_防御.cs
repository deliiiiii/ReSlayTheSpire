using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(100)][Serializable]
public class Card100 : CardInTurn
{
    int BlockValue => BlockAt(0);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.GainBlock(BlockValue);
        return UniTask.CompletedTask;
    }
}