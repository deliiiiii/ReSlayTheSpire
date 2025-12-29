using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(21)][Serializable]
public class Card21 : Card
{
    int Atk => AtkAt(0);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        return UniTask.CompletedTask;
    }
}