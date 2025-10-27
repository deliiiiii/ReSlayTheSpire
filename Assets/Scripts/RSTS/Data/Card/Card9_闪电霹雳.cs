﻿using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(9)][Serializable]
public class Card9 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    BuffDataVulnerable buffVulnerable => NthEmbedAsBuffCopy<BuffDataVulnerable>(1);
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackAllEnemies(atk);
        bothTurnData.AddBuffToAllEnemies(() => buffVulnerable);
        return UniTask.CompletedTask;
    }
}
