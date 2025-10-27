using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(11)][Serializable]
public class Card11 : CardDataBase
{
    int atk => EmbedInt(0);
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.AddTempToDraw(() => CreateCard(5004));
        
        return UniTask.CompletedTask;
    }
}
