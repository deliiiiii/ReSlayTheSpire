using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(3)][Serializable]
public class Card3 : Card
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        bothTurn.OpenDiscardOnceClick(cardData =>
        {
            bothTurn.DiscardList.MyRemove(cardData);
            bothTurn.DrawList.MyAdd(cardData);
        });
        return UniTask.CompletedTask;
    }
}