using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(9)][Serializable]
public class Card9 : Card
{
    int Atk => AtkAt(0);
    BuffDataVulnerable BuffVulnerable => BuffAt<BuffDataVulnerable>(1);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackAllEnemies(Atk);
        bothTurn.AddBuffToAllEnemies(BuffVulnerable);
        return UniTask.CompletedTask;
    }
}
