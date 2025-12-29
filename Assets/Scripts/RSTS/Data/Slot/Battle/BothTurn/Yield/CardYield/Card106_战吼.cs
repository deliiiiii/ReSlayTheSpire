using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(106)][Serializable]
public class Card106 : Card
{
    int Draw => DrawAt(0);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.DrawSome(Draw);
        bothTurn.OpenHandCardOnceClick(1,
            handCard => handCard != this,
            handCard =>
            {
                bothTurn.HandList.MyRemove(handCard);
                bothTurn.DrawList.MyAdd(handCard);
            });
        return UniTask.CompletedTask;
    }
}