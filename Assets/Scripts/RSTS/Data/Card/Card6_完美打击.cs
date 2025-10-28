using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(6)][Serializable]
public class Card6 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    int addPerDaJi => NthEmbedAs<EmbedMisc>(1).MiscValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk, GetModifyList(bothTurnData));
        return UniTask.CompletedTask;
    }

    public override List<AttackModifyBase> GetModifyList(BothTurnData bothTurnData)
        => [new AttackModifyCard6() { BaseAtkAddByDaJi = addPerDaJi * bothTurnData.DaJiCount }];
}