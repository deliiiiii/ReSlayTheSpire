using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(90)][Serializable]
public class Card90 : CardInTurn
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        return UniTask.CompletedTask;
    }
}