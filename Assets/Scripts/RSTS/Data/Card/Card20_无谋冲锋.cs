using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(20)][Serializable]
public class Card20 : CardInTurn
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.AddTempToDraw(() => CreateBlindCard(5003));
        
        return UniTask.CompletedTask;
    }
}
