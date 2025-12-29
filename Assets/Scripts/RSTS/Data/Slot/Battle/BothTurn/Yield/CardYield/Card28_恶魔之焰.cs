using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;

namespace RSTS;
[Card(28)][Serializable]
public class Card28 : Card
{
    int Atk => AtkAt(0);
    List<Card> ToExhaustCards(BothTurn bothTurn) => bothTurn.HandList.Where(handCard => handCard != this).ToList();
    public override async UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        ToExhaustCards(bothTurn).ForEach(handCard =>
        {
            bothTurn.HandList.MyRemove(handCard);
            bothTurn.ExhaustList.MyAdd(handCard);
        });
        await bothTurn.AttackEnemyMultiTimesAsync(target, Atk, ToExhaustCards(bothTurn).Count);
    }

    protected override List<AttackModifyBase> GetModifyList(BothTurn bothTurn) =>
    [
        new AttackModifyCard28 { AtkTimeByExhaust = ToExhaustCards(bothTurn).Count },
    ];
}