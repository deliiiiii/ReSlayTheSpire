using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace RSTS;
[CardID(13)][Serializable]
public class Card13 : CardDataBase
{
    int atk => NthEmbedAs<EmbedAttack>(0).AttackValue;
    public override UniTask YieldAsync(BothTurnData bothTurnData, int costEnergy)
    {
        bothTurnData.AttackEnemy(Target, atk);
        return UniTask.CompletedTask;
    }

    public override bool YieldCondition(BothTurnData bothTurnData, out string failReason)
    {
        failReason = string.Empty;
        if (bothTurnData.HandList.All(cardData => cardData.Config.Category == ECardCategory.Attack))
            return true;
        failReason = "我的手牌中有非攻击牌！";
        return false;
    }
}
