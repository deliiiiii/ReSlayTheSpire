using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(3)][Serializable]
public class Card3 : CardInTurn
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        BothTurn.OpenDiscardOnceClick(cardData =>
        {
            BothTurn.DiscardList.MyRemove(cardData);
            BothTurn.DrawList.MyAdd(cardData);
        });
        return UniTask.CompletedTask;
    }
}