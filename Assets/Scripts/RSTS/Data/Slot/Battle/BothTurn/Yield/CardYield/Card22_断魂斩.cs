using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(22)][Serializable]
public class Card22 : Card
{
    int Atk => AtkAt(0);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        bothTurn.HandList
            .Where(handCard => handCard.Config.Category != ECardCategory.Attack)
            .ToList()
            .ForEach(handCard =>
            {
                bothTurn.HandList.MyRemove(handCard);
                bothTurn.ExhaustList.MyAdd(handCard);
            });
        return UniTask.CompletedTask;
    }
}