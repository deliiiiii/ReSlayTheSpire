using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;

namespace RSTS;
[CardID(28)][Serializable]
public class Card28 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    IEnumerable<CardDataBase> ToExhaustCards(BothTurnData bothTurnData) 
        => bothTurnData.ExhaustHandCardBy(cardData => cardData != this);
    public override async UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        var count = ToExhaustCards(bothTurnData).Count();
        ToExhaustCards(bothTurnData).ForEach(bothTurnData.ExhaustOne);
        await bothTurnData.AttackEnemyMultiTimesAsync(Target, atk, count);
    }

    public override List<AttackModifyBase> GetModifyList(BothTurnData bothTurnData) =>
        [
            new AttackModifyCard28
            {
                AtkTimeByExhaust = ToExhaustCards(bothTurnData).Count()
            }
        ];
}