using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(4)][Serializable]
public class Card4 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    public int StrengthMulti => NthEmbedAs<EmbedMisc>(1).MiscValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk, StrengthMulti);
        return UniTask.CompletedTask;
    }
}