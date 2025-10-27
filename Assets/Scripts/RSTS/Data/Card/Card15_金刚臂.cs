using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(15)][Serializable]
public class Card15 : CardDataBase
{
    int atk => EmbedInt(0);
    int weakCount => EmbedInt(1);

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.AddBuffToEnemy(Target, new BuffDataWeak(weakCount));
        return UniTask.CompletedTask;
    }
}