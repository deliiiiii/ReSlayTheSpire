using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;

namespace RSTS;
[Card(28)][Serializable]
public class Card28 : CardInTurn
{
    int Atk => AtkAt(0);
    IEnumerable<Card> ToExhaustCards() => BothTurn.HandList.Where(handCard => handCard != Card);
    public override async UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        ToExhaustCards().ForEach(handCard =>
        {
            BothTurn.HandList.MyRemove(handCard);
            BothTurn.ExhaustList.MyAdd(handCard);
        });
        await BothTurn.AttackEnemyMultiTimesAsync(target, Atk, ToExhaustCards().Count());
    }

    protected override List<AttackModifyBase> ModifyList =>
        [
            new AttackModifyCard28 { AtkTimeByExhaust = ToExhaustCards().Count() },
        ];
}