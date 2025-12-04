using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;

namespace RSTS;
[CardID(28)][Serializable]
public class Card28(CardData parent) : CardInTurn(parent)
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    List<CardInTurn> ToExhaustCards(BothTurnData bothTurnData) 
        => bothTurnData.HandList.Where(cardData => cardData != this).ToList();
    public override async UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        var count = ToExhaustCards(bothTurnData).Count;
        ToExhaustCards(bothTurnData).ForEach(handCard =>
        {
            bothTurnData.HandList.MyRemove(handCard);
            bothTurnData.ExhaustList.MyAdd(handCard);
        });
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