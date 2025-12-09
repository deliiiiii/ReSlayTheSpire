using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(115)][Serializable]
public class Card115 : CardInTurn
{
    BuffDataAttackGainBlock BuffAttackGainBlock => BuffAt<BuffDataAttackGainBlock>(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AddBuffToPlayer(BuffAttackGainBlock);
        return UniTask.CompletedTask;
    }
}