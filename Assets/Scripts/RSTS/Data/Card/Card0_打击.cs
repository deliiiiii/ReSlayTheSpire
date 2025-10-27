using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(0)][Serializable]
public class Card0 : CardDataBase
{
    int atk => EmbedInt(0);

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        return UniTask.CompletedTask;
    }
}