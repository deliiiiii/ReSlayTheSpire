using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(27)][Serializable]
public class Card27 : CardInTurn
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    int addHPMax => NthEmbedAs<EmbedMisc>(1).MiscValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemyWithResult(Target, atk, out var resultList);
        if (resultList.OfType<AttackResultDie>().Any())
        {
            bothTurnData.GainMaxHP(addHPMax);
        }
        return UniTask.CompletedTask;
    }
}