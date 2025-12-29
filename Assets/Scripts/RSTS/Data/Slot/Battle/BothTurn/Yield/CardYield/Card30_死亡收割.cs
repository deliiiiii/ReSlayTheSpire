using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace RSTS;
[Card(30)][Serializable]
public class Card30 : Card
{
    int Atk => AtkAt(0);
    public override UniTask YieldAsync(BothTurn bothTurn, int cost, EnemyDataBase? target)
    {
        var gainHPSum = bothTurn.AttackAllEnemies(Atk).OfType<AttackResultHurt>().Sum(r => r.HurtDamage);
        bothTurn.GainCurHP(gainHPSum);
        return UniTask.CompletedTask;
    }
}