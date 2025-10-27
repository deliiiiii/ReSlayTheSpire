using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(4)][Serializable]
public class Card4 : CardDataBase
{
    int atk => EmbedInt(0);
    int strengthMulti => EmbedInt(1);

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk, strengthMulti);
        return UniTask.CompletedTask;
    }
}