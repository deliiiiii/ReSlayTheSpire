using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(7)][Serializable]
public class Card7 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    int draw => NthEmbedAs<EmbedDraw>(1).DrawValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.DrawSome(draw);
        return UniTask.CompletedTask;
    }
}
