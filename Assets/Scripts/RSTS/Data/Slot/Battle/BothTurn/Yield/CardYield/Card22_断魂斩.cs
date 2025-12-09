using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(22)][Serializable]
public class Card22 : CardInTurn
{
    int Atk => AtkAt(0);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        BothTurn.HandList
            .Where(handCard => handCard.Config.Category != ECardCategory.Attack)
            .ToList()
            .ForEach(handCard =>
            {
                BothTurn.HandList.MyRemove(handCard);
                BothTurn.ExhaustList.MyAdd(handCard);
            });
        return UniTask.CompletedTask;
    }
}