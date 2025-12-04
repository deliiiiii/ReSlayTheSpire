using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(4)][Serializable]
public class Card4(CardData parent) : CardInTurn(parent)
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    public int StrengthMulti => NthEmbedAs<EmbedMisc>(1).MiscValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk, GetModifyList(bothTurnData));
        return UniTask.CompletedTask;
    }

    public override List<AttackModifyBase> GetModifyList(BothTurnData bothTurnData)
        => [new AttackModifyCard4 { StrengthMulti = StrengthMulti }];
}