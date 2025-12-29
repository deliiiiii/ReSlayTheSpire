using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(16)][Serializable]
public class Card16 : Card
{
    int Atk => AtkAt(0);
    int GainEnergy => EnergyAt(1);
    int Draw => DrawAt(2);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        // if (RecommendYield(Outer))
        if(target?.HPAndBuffData.HasBuff<BuffDataVulnerable>(out _) ?? false)
        {
            bothTurn.GainEnergy(GainEnergy);
            bothTurn.DrawSome(Draw);
        }
        return UniTask.CompletedTask;
    }

    // public override bool RecommendYield(BothTurnData bothTurnData)
    // {
    //     return target.HPAndBuffData.HasBuff<BuffDataWeak>(out _);
    // }
}