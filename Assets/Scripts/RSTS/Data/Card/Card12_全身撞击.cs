using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(12)][Serializable]
public class Card12 : CardDataBase
{
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, 0, GetModifyList(bothTurnData));
        return UniTask.CompletedTask;
    }

    public override List<AttackModifyBase> GetModifyList(BothTurnData bothTurnData)
        => [new AttackModifyCard12() { AtkByBlock = bothTurnData.PlayerBlock }];
}
