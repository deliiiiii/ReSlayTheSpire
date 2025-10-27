using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(7)][Serializable]
public class Card7 : CardDataBase
{
    int atk => EmbedInt(0);
    int draw => EmbedInt(1);
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.DrawSome(draw);
        return UniTask.CompletedTask;
    }
}
