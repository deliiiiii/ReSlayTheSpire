using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(203)][Serializable]
public class Card203 : CardInTurn
{
    BuffDataStrength BuffStrength => BuffAt<BuffDataStrength>(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AddBuffToPlayer(BuffStrength);
        return UniTask.CompletedTask;
    }
}