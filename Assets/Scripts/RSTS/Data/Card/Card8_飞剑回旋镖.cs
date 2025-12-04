using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(8)][Serializable]
public class Card8(CardData parent) : CardInTurn(parent)
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    int atkTime => NthEmbedAs<EmbedMisc>(1).MiscValue;
    public override async UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        await bothTurnData.AttackEnemyRandomlyMultiTimesAsync(atk, atkTime);
    }
}
