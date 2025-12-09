using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(2)][Serializable]
public class Card2 : CardInTurn
{
    int Atk => AtkAt(0);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        BothTurn.AddTempToDiscard(Card.DeepCopy());
        return UniTask.CompletedTask;
    }
}