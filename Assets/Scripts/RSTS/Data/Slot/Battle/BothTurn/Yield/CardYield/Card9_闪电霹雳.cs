using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(9)][Serializable]
public class Card9 : CardInTurn
{
    int Atk => AtkAt(0);
    BuffDataVulnerable BuffVulnerable => BuffAt<BuffDataVulnerable>(1);
    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackAllEnemies(Atk);
        BothTurn.AddBuffToAllEnemies(() => BuffVulnerable);
        return UniTask.CompletedTask;
    }
}
