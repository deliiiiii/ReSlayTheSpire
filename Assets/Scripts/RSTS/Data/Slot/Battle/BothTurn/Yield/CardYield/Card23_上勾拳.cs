using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(23)][Serializable]
public class Card23 : Card
{
    int Atk => AtkAt(0);
    BuffDataWeak BuffWeak => BuffAt<BuffDataWeak>(1);
    BuffDataVulnerable BuffVulnerable => BuffAt<BuffDataVulnerable>(2);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        bothTurn.AddBuffToEnemy(target, BuffWeak);
        bothTurn.AddBuffToEnemy(target, BuffVulnerable);
        return UniTask.CompletedTask;
    }
}