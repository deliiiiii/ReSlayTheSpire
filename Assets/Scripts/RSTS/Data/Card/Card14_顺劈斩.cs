using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(14)][Serializable]
public class Card14(CardData parent) : CardInTurn(parent)
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackAllEnemies(atk);
        return UniTask.CompletedTask;
    }
}
