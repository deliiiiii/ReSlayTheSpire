using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(106)][Serializable]
public class Card106 : CardInTurn
{
    int Draw => DrawAt(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.DrawSome(Draw);
        BothTurn.OpenHandCardOnceClick(1,
            handCard => handCard != Card,
            handCard =>
            {
                BothTurn.HandList.MyRemove(handCard);
                BothTurn.DrawList.MyAdd(handCard);
            });
        return UniTask.CompletedTask;
    }
}