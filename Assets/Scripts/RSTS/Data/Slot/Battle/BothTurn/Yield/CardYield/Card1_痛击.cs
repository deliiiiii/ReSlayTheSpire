using System;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(1)][Serializable]
public class Card1 : CardInTurn
{
    int Atk => AtkAt(0);
    BuffDataVulnerable VulnerableBuff => BuffAt<BuffDataVulnerable>(1);

    public override UniTask YieldAsync(int cost, EnemyDataBase? target)
    {
        BothTurn.AttackEnemy(target, Atk);
        BothTurn.AddBuffToEnemy(target, VulnerableBuff);
        return UniTask.CompletedTask;
    }
}