using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(23)][Serializable]
public class Card23 : CardInTurn
{
    int Atk => AtkAt(0);
    BuffDataWeak BuffWeak => BuffAt<BuffDataWeak>(1);
    BuffDataVulnerable BuffVulnerable => BuffAt<BuffDataVulnerable>(2);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        BothTurn.AddBuffToEnemy(target, BuffWeak);
        BothTurn.AddBuffToEnemy(target, BuffVulnerable);
        return UniTask.CompletedTask;
    }
}