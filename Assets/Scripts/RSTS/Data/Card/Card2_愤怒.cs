using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(2)][Serializable]
public class Card2 : CardDataBase
{
    int atk => EmbedInt(0);

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        var copied = this.DeepCopy();
        bothTurnData.AddTempToDiscard(copied);
        return UniTask.CompletedTask;
    }
}