using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(2)][Serializable]
public class Card2 : Card
{
    int Atk => AtkAt(0);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        bothTurn.AddTempToDiscard(this.DeepCopy());
        return UniTask.CompletedTask;
    }
}