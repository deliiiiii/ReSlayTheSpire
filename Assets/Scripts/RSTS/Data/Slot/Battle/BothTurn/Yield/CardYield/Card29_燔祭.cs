using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(29)][Serializable]
public class Card29 : Card
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackAllEnemies(Atk);
        bothTurn.AddTempToDiscard(Create(5002));
        return UniTask.CompletedTask;
    }
}