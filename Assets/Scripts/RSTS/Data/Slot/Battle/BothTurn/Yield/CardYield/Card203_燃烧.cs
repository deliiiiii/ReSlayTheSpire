using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(203)][Serializable]
public class Card203 : Card
{
    BuffDataStrength BuffStrength => BuffAt<BuffDataStrength>(0);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AddBuffToPlayer(BuffStrength);
        return UniTask.CompletedTask;
    }
}