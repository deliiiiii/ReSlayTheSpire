using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(20)][Serializable]
public class Card20 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        bothTurnData.AddTempToDraw(() => CreateData(5003));
        
        return UniTask.CompletedTask;
    }
}
