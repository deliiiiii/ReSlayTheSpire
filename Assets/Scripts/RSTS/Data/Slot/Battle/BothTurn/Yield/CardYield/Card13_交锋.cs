using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(13)][Serializable]
public class Card13 : CardInTurn
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        return UniTask.CompletedTask;
    }

    public override bool YieldCondition(out string failReason)
    {
        failReason = string.Empty;
        if (BothTurn.HandList.All(card => card.Config.Category == ECardCategory.Attack))
            return true;
        failReason = "我的手牌中有非攻击牌！";
        return false;
    }
}
