using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(11)][Serializable]
public class Card11 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.AddTempToDraw(() => CreateData(5004));
        
        return UniTask.CompletedTask;
    }
}
