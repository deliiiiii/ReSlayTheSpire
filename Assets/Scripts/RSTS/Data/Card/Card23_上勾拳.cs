using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(23)][Serializable]
public class Card23(CardData parent) : CardInTurn(parent)
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    BuffDataWeak buffWeak => NthEmbedAsBuffCopy<BuffDataWeak>(1);
    BuffDataVulnerable buffVulnerable => NthEmbedAsBuffCopy<BuffDataVulnerable>(2);

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.AddBuffToEnemy(Target, buffWeak);
        bothTurnData.AddBuffToEnemy(Target, buffVulnerable);
        return UniTask.CompletedTask;
    }
}