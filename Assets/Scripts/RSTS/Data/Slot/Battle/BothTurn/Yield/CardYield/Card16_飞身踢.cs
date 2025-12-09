using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(16)][Serializable]
public class Card16 : CardInTurn
{
    int Atk => AtkAt(0);
    int GainEnergy => EnergyAt(1);
    int Draw => DrawAt(2);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        // if (RecommendYield(Outer))
        if(target?.HPAndBuffData.HasBuff<BuffDataVulnerable>(out _) ?? false)
        {
            BothTurn.GainEnergy(GainEnergy);
            BothTurn.DrawSome(Draw);
        }
        return UniTask.CompletedTask;
    }

    // public override bool RecommendYield(BothTurnData bothTurnData)
    // {
    //     return target.HPAndBuffData.HasBuff<BuffDataWeak>(out _);
    // }
}