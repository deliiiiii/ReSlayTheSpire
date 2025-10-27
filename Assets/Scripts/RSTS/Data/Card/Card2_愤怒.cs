using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(2)][Serializable]
public class Card2 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;

    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.AddTempToDiscard(() => this.DeepCopy());
        return UniTask.CompletedTask;
    }
}