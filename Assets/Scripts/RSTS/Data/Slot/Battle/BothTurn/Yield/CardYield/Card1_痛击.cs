using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(1)][Serializable]
public class Card1 : Card
{
    int Atk => AtkAt(0);
    BuffDataVulnerable VulnerableBuff => BuffAt<BuffDataVulnerable>(1);

    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        bothTurn.AttackEnemy(target, Atk);
        bothTurn.AddBuffToEnemy(target, VulnerableBuff);
        return UniTask.CompletedTask;
    }
}