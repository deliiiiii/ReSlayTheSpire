using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(1)][Serializable]
public class Card1(CardData parent) : CardInTurn(parent)
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    BuffDataVulnerable vulnerableBuff => NthEmbedAsBuffCopy<BuffDataVulnerable>(1);

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.AddBuffToEnemy(Target, vulnerableBuff);
        return UniTask.CompletedTask;
    }
}