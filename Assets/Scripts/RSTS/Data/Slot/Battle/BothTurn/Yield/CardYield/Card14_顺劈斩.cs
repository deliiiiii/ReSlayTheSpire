using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(14)][Serializable]
public class Card14 : Card
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackAllEnemies(Atk);
        return UniTask.CompletedTask;
    }
}
