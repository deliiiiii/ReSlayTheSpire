using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(29)][Serializable]
public class Card29 : CardInTurn
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackAllEnemies(Atk);
        BothTurn.AddTempToDiscard(CreateNowhereCard(5002, BothTurn));
        return UniTask.CompletedTask;
    }
}