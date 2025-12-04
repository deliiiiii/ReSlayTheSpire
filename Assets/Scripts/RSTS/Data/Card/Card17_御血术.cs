using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(17)][Serializable]
public class Card17(CardData parent) : CardInTurn(parent)
{
    int loseHP => NthEmbedAs<EmbedMisc>(0).MiscValue;
    int atk => NthEmbedAs<EmbedAttack>(1).AttackValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.LoseHP(loseHP);
        bothTurnData.AttackEnemy(Target, atk);
        return UniTask.CompletedTask;
    }
}