using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(30)][Serializable]
public class Card30(CardData parent) : CardInTurn(parent)
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackAllEnemiesWithResult(atk, out var resultList);
        var gainHPSum = resultList.OfType<AttackResultHurt>().Sum(r => r.HurtDamage);
        bothTurnData.GainCurHP(gainHPSum);
        return UniTask.CompletedTask;
    }
}