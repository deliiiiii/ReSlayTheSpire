using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(17)][Serializable]
public class Card17 : CardDataBase
{
    int loseHP => EmbedInt(0);
    int atk => EmbedInt(1);

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.LoseHPToPlayer(loseHP);
        bothTurnData.AttackEnemy(Target, atk);
        return UniTask.CompletedTask;
    }
}