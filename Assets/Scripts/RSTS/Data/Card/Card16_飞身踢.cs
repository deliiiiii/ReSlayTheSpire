using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(16)][Serializable]
public class Card16: CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    int energy => NthEmbedAs<EmbedEnergy>(1).EnergyValue;
    int card => NthEmbedAs<EmbedDraw>(2).DrawValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        // if (RecommendYield(bothTurnData))
        if(Target.HPAndBuffData.HasBuff<BuffDataVulnerable>(out _))
        {
            bothTurnData.GainEnergy(energy);
            bothTurnData.DrawSome(card);
        }
        return UniTask.CompletedTask;
    }

    // public override bool RecommendYield(BothTurnData bothTurnData)
    // {
    //     return Target.HPAndBuffData.HasBuff<BuffDataWeak>(out _);
    // }
}