using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(90)][Serializable]
public class Card90 : CardDataBase
{
    int atk => EmbedInt(0);

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        return UniTask.CompletedTask;
    }
}