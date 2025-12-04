using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(10)][Serializable]
public class Card10(CardData parent) : CardInTurn(parent)
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    public override async UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        await bothTurnData.AttackEnemyMultiTimesAsync(Target, atk, 2);
    }
}
