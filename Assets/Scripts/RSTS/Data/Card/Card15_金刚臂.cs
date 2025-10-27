using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(15)][Serializable]
public class Card15 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    BuffDataWeak weakBuff => NthEmbedAsBuffCopy<BuffDataWeak>(1);

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.AddBuffToEnemy(Target, weakBuff);
        return UniTask.CompletedTask;
    }
}